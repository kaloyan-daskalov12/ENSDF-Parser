# ENSDF Parser

ENSDF-Parser is a C# library and interactive console application for working with Evaluated Nuclear Structure Data File (ENSDF) records.

The library parses fixed-column text into structured objects representing datasets, nuclear levels, radiation, comments, and normalization data. The console application provides commands for loading files, navigating these objects, editing record fields, and saving datasets. A separate extraction feature reads numerical values and uncertainties from gamma comments.

## Contents

- [Build and Run](#build-and-run)
- [Solution Structure](#solution-structure)
- [Parsing Flow](#parsing-flow)
- [Object Model](#object-model)
- [Supported Record Types](#supported-record-types)
- [Fixed-Column Parsing and Serialization](#fixed-column-parsing-and-serialization)
- [Gamma-Comment Value Extraction](#gamma-comment-value-extraction)
- [Console Application Command Reference](#console-application-command-reference)
- [Using the Library](#using-the-library)
- [Contributing](#contributing)

## Build and Run

Install the **.NET 10 SDK**, then execute these commands from the repository root:

```powershell
dotnet restore ENSDF_Parser.slnx
dotnet build ENSDF_Parser.slnx
dotnet run --project ENSDF_Parser_App/ENSDF_Parser_App.csproj
```

The application starts an interactive session. Enter application commands at its prompt:

```text
parse file "C:\data\sample.ens"
dir
select dataset[0]
dir
get Header.IdentificationRecords[0].DSID
exit
```

Replace the example path with an existing ENSDF file. This example assumes the file contains a dataset with an identification record.

## Solution Structure

The solution contains two projects, both targeting `net10.0`.

```text
ENSDF_Parser.slnx
│
├── ENSDF_Parser/
│   ├── Parser.cs
│   ├── DataSet.cs
│   ├── Records/
│   │   └── Base/Record.cs
│   ├── MetaData/
│   └── Extractor/
│
└── ENSDF_Parser_App/
    ├── Program.cs
    ├── UIApp.cs
    ├── InputParser.cs
    ├── Session.cs
    ├── Selection.cs
    └── Tools.cs
```

### Parser Library

`ENSDF_Parser` contains:

| Component | Responsibility |
|---|---|
| `Parser` | Recognizes input records and separates datasets. |
| `DataSet` | Organizes records into headers, levels, and radiation collections. |
| `Records/` | Defines individual ENSDF record classes and their text representations. |
| `MetaData/` | Defines identifiers and structural containers such as `Header` and `Level`. |
| `Extractor/` | Extracts gamma-comment values and interprets their uncertainties. |

The library accepts in-memory input lines. Applications using it control file access.

### Console Application

`ENSDF_Parser_App` references the library and adds an interactive interface:

| Component | Responsibility |
|---|---|
| `Program` | Starts the application. |
| `UIApp` | Runs the prompt loop and displays errors. |
| `InputParser` | Dispatches commands. |
| `Session` | Manages datasets, selections, extraction results, and file operations. |
| `Selection` | Navigates objects and lists through reflection. |
| `Tools` | Parses command arguments and formats extracted values. |

Neither project declares external package dependencies.

## Parsing Flow

Parsing proceeds in two stages: recognizing individual records and organizing them into datasets.

1. The caller supplies a list of text lines and a source-file label.
2. Empty or space-only lines separate datasets.
3. Nonblank records are checked for a minimum length of 80 characters before field extraction.
4. Columns 1–5 are parsed into a nuclide identifier.
5. Columns 6–9 determine the record type.
6. The selected record constructor reads its fields using fixed character offsets.
7. The dataset distributes records into its header, levels, radiation collections, and comment associations.
8. Any accumulated records remaining at the end of input form the final dataset.

Radiation records appearing before the first level are stored in `UnplacedRecords`. Once a level is encountered, subsequent radiation records are associated with the current level.

Parent and normalization associations use state accumulated while reading the dataset. Recognized continuation records are represented as `CommentRecord` objects.

Unrecognized record types are retained as base `Record` objects containing their original text.

## Object Model

```text
DataSet
├── Id
├── File
├── Header
│   ├── IdentificationRecords
│   ├── HistoryRecords
│   ├── CrossReferenceRecords
│   ├── QValueRecords
│   ├── CommentRecords
│   ├── PN_Pairs
│   ├── UnpairedNormalizationRecords
│   └── UnpairedProductionNormalizationRecords
├── UnplacedRecords
├── Levels
│   └── Level
│       ├── LevelRecord
│       └── Data
│           ├── GammaRecords
│           ├── BettaRecords
│           ├── ECRecords
│           ├── AlphaRecords
│           └── DelayedParticleRecords
└── OrderedRecords (private)
```

### Core Types

| Type | Description |
|---|---|
| `DataSet` | Contains a dataset’s identity, source label, grouped records, and original record sequence. |
| `Header` | Collects dataset metadata and parent/normalization associations. |
| `Level` | Combines a `LevelRecord` with its radiation data. |
| `RadiationData` | Holds separate collections for each radiation type. |
| `PNPair` | Groups parent, normalization, and production-normalization records. |
| `Record` | Common base with `Id`, `RType`, and a `Comments` collection. |
| `Identifier` | Holds the mass identifier in `Isotrope` and the element identifier in `Name`, both strings. |
| `RecordType` | Represents columns 6–9 through `C1`, `C2`, `C3`, and `C4`. |
| `Value` | Holds an extracted numerical value and its absolute uncertainty. |

`Isotrope` and `BettaRecords` are the current public API spellings.

Most ENSDF record fields remain strings. For example, `GammaRecord.E`, `DE`, `RI`, and `M` retain the field’s textual value. Property setters trim surrounding whitespace.

## Supported Record Types

The following table summarizes the classes recognized by the implementation. Single-letter markers refer to column 8 unless otherwise stated.

| Marker | Record class | Main content |
|---|---|---|
| Blank columns 6–9 | `IdentificationRecord` | Dataset identification, references, publication, date |
| `L` | `LevelRecord` | Level energy, spin/parity, lifetime, related fields |
| `G` | `GammaRecord` | Gamma energy, intensity, multipolarity, conversion data |
| `B` | `BetaRecord` | Beta energy, intensity, log ft |
| `E` | `ECRecord` | Electron-capture and positron data |
| `A` | `AlphaRecord` | Alpha energy, intensity, hindrance factor |
| `P` | `ParentRecord` | Parent energy, spin/parity, lifetime, Q value |
| `N` | `NormalizationRecord` | Normalization factors and branching ratio |
| `PN` in columns 7–8 | `ProductionNormalizationRecord` | Combined normalization factors and options |
| `Q` | `QValueRecord` | Q values, separation energies, references |
| `X` | `CrossReferenceRecord` | Cross-reference symbol and dataset identifier |
| `H` | `HistoryRecord` | History text |
| Blank or `D` in column 8, particle symbol in column 9 | `DelayedParticleRecord` | Prompt or delayed particle data |
| `C`, `D`, `T`, `c`, or `t` in column 7 | `CommentRecord` | Comment text |
| Unrecognized pattern | `Record` | Original input text |

The particle symbols recognized in column 9 are `N`, `P`, `A`, `D`, and `T`.

Identification recognition also accepts a digit in column 6 when columns 7–9 are blank.

A continuation is represented as `CommentRecord` when:

- Column 6 is neither a space nor `1`.
- Columns 7 and 9 are blank.
- Column 8 is `L`, `B`, `E`, `G`, or `H`.

## Fixed-Column Parsing and Serialization

ENSDF records use positional fields. Column numbers below are **one-based and inclusive**; C# string offsets are zero-based.

| Columns | Content |
|---|---|
| 1–3 | Mass identifier |
| 4–5 | Element identifier |
| 6–9 | Record discriminator |
| 10–80 | Record-specific fields |

For example:

```csharp
line.Substring(9, 10)
```

reads columns 10–19.

Preserve spaces when preparing input. They determine field positions.

### Example: Gamma Record

| Columns | Field |
|---|---|
| 10–19 | `E` |
| 20–21 | `DE` |
| 22–29 | `RI` |
| 30–31 | `DRI` |
| 32–41 | `M` |
| 42–49 | `MR` |
| 50–55 | `DMR` |
| 56–62 | `CC` |
| 63–64 | `DCC` |
| 65–74 | `TI` |
| 75–76 | `DTI` |
| 77 | `FLAG` |
| 78 | `COIN` |
| 79 | Reserved space |
| 80 | `Q` |

Each record class defines its input slices and output layout.

### Formatting

Record serializers construct text through `ToString()`:

- The mass identifier is padded on the left to three characters.
- The element identifier is padded on the right to two characters.
- Individual fields are padded on the right to their assigned widths.
- Values exceeding a field width are truncated by `FormatToNChars`.
- Reserved regions are written as spaces.
- Base records representing unrecognized input return their original text.

`DataSet.ToString()` joins records with LF line endings and appends a line of 80 spaces followed by LF as the dataset terminator.

### Original Order and Grouped Records

`DataSet` maintains both an original sequence and a grouped view.

`OrderedRecords` is the private sequence used for serialization. `Header`, `Levels`, and radiation collections provide structured access to the same record instances.

Editing an existing record through the grouped model therefore updates its serialized fields. Structural collection edits and the original sequence are managed separately; adding or removing grouped items does not rebuild `OrderedRecords`.

Parsed comments are retained in the original sequence and associated with objects for navigation and extraction. Dataset serialization writes them from the original sequence.

## Gamma-Comment Value Extraction

**GCV** stands for **Gamma Comment Values**.

`ValueExtractor.ParseFromComments` processes lowercase-`c` comments attached to gamma records under dataset levels. It returns:

- Extracted values grouped by level and gamma record.
- Diagnostic messages encountered during extraction.

### Markers and Breakers

An identifier followed by `$` marks the text to extract. For an identifier of `E`, an illustrative comment fragment is:

```text
E$ 12.34 {I5} 56.78 {I9} STOP
```

The caller supplies breaker strings such as `STOP`. Extraction state allows multiple comment records to contribute values. A different `$` marker or a supplied breaker ends the active extraction section.

Pass the identifier without `$` when calling the extractor or console command.

### Values and Uncertainties

`Value.Val` stores the numerical value. `Value.DVal` stores its absolute uncertainty, scaled according to the decimal precision of the input.

For decimal-point input:

| Text | `Val` | `DVal` |
|---|---|---|
| `12.34 {I5}` | `12.34` | `0.05` |
| `56.78 {I9}` | `56.78` | `0.09` |

Numerical parsing and output use the process’s current culture.

A numerical uncertainty without `{I...}` markup is accepted and recorded in the diagnostic messages. Optional filtering removes entries with zero or missing uncertainty.

### Export Templates

Templates use two replacement strings:

| Placeholder | Replacement |
|---|---|
| `val` | Numerical value |
| `dval` | Uncertainty digits returned by `Value.GetString()` |

For `12.34 {I5}`, the template:

```text
val dval
```

produces:

```text
12.34 5
```

Here, `dval` is `5`; the corresponding absolute `DVal` is `0.05`.

Each extracted value produces one output line.

## Console Application Command Reference

The console provides a session containing loaded datasets, a current selection, and stored extraction results.

### Command Conventions

- Enter commands at the application prompt without a leading hyphen.
- Commands and property names are case-sensitive.
- Indices start at zero.
- Use dots to traverse properties.
- Use brackets to select list elements.
- Use comma-separated indices for commands accepting multiple datasets.
- Quote file paths and export templates containing spaces.
- Paths beginning with `dataset[index]` start from the session’s dataset list.
- Other property paths are relative to the current selection.

For example:

```text
dataset[0].Levels[1].Data.GammaRecords[2].E
```

identifies the energy field of the third gamma record in the second level of the first dataset.

The examples below assume the referenced datasets and records exist.

### Command Summary

| Command | Purpose |
|---|---|
| `parse file` | Load datasets from a file |
| `dir` | Inspect the current selection |
| `select` | Change the current selection |
| `get` | Read a property |
| `set` | Edit a writable record property |
| `save dataset[...]` | Write selected datasets |
| `parse gcv` | Extract gamma-comment values |
| `save gcv` | Export stored values and diagnostics |
| `exit` | End the session |

### `parse file`: Load ENSDF Data

```text
parse file "C:\data\sample.ens"
```

Loads the file and appends its datasets to the session. Loading another file retains the datasets already loaded.

The input filename without its extension becomes each dataset’s `File` label.

### `dir`: Inspect the Current Selection

```text
dir
```

For an object, displays its public properties and their types. For a list, displays its element count.

Example:

```text
select dataset[0]
dir
```

### `select`: Navigate the Object Model

Select an object using an absolute dataset path:

```text
select dataset[0].Levels[0].Data.GammaRecords[0]
```

Or navigate relative to the current selection:

```text
select dataset[0]
select Levels[0]
select Data.GammaRecords[0]
```

Move back through the selection chain:

```text
select ..
```

The prompt shows the current selection.

### `get`: Read a Property

Read a property using a full path:

```text
get dataset[0].Header.IdentificationRecords[0].DSID
get dataset[0].Levels.Count
get dataset[0].Levels[0].Data.GammaRecords[0].E
```

After selecting a record, use its property name:

```text
select dataset[0].Levels[0].Data.GammaRecords[0]
get E
get DE
get RI
```

### `set`: Edit a Record Field

Assign a new string value to a writable field property:

```text
select dataset[0].Levels[0].Data.GammaRecords[0]
set E 123.45
set DE 6
get E
```

A full path can also be used:

```text
set dataset[0].Levels[0].Data.GammaRecords[0].RI 25.0
```

Changes remain in memory until the dataset is saved.

### `save dataset[...]`: Save ENSDF Data

Save one dataset:

```text
save dataset[0] "C:\data\edited.ens"
```

Save several datasets in the specified order:

```text
save dataset[0,2] "C:\data\selected.ens"
```

The command writes each dataset’s serialized record sequence. An existing destination file is overwritten. The destination directory should already exist.

### `parse gcv`: Extract Gamma-Comment Values

```text
parse gcv dataset[0] E [STOP]
```

Arguments are:

| Argument | Meaning |
|---|---|
| `dataset[0]` | Dataset indices to process |
| `E` | Identifier matching `E$` in comment text |
| `[STOP]` | Comma-separated breaker strings |
| Optional trailing `r` | Remove values with zero or missing uncertainty |

Examples:

```text
parse gcv dataset[0] E [STOP] r
```

```text
parse gcv dataset[0,1] E [STOP,END]
```

```text
parse gcv dataset[0] E []
```

The last example supplies no breaker strings. The optional `r` is written without brackets.

Results are stored in the session for subsequent export.

### `save gcv`: Export Extracted Values

```text
save gcv "C:\data\extracted" "val dval"
```

Exports the session’s stored extraction results using the supplied template. Output directories are created automatically.

The directory structure is:

```text
extracted/
├── Values/
│   └── Dataset-<File>/
│       └── Level-<level energy>/
│           └── Gamma-<gamma energy>.txt
└── Logs/
    └── Dataset-<File>.txt
```

`<File>` is the dataset’s source-file label. Value files contain formatted extraction results; log files contain diagnostic messages.

### `exit`: End the Session

```text
exit
```

Ends the interactive session. Save any desired results before exiting.

### Example Workflow: Inspect, Edit, and Save

```text
parse file "C:\data\sample.ens"
select dataset[0]
get Header.IdentificationRecords[0].DSID
get Levels.Count
select Levels[0].Data.GammaRecords[0]
dir
get E
set E 123.45
set DE 6
save dataset[0] "C:\data\edited.ens"
exit
```

### Example Workflow: Extract Comment Values

For a file containing comments using `E$` and `STOP`:

```text
parse file "C:\data\sample.ens"
parse gcv dataset[0] E [STOP] r
save gcv "C:\data\extracted" "val dval"
exit
```

## Using the Library

Reference `ENSDF_Parser/ENSDF_Parser.csproj` from a compatible .NET application.

### Parse and Inspect Datasets

```csharp
using System;
using System.IO;
using System.Linq;
using ENSDF_Parser;

string path = "sample.ens";
var lines = File.ReadAllLines(path).ToList();

var datasets = Parser.Parse(
    lines,
    Path.GetFileNameWithoutExtension(path));

foreach (var dataset in datasets)
{
    Console.WriteLine(dataset.GetOverview());

    foreach (var level in dataset.Levels)
    {
        foreach (var gamma in level.Data.GammaRecords)
        {
            Console.WriteLine(
                $"Level {level.LevelRecord.E}: gamma {gamma.E}");
        }
    }
}
```

The second argument to `Parser.Parse` is a label stored on the datasets. The parser does not open that path itself.

### Edit and Serialize

Using the `datasets` list from the previous example:

```csharp
var gamma = datasets
    .SelectMany(dataset => dataset.Levels)
    .SelectMany(level => level.Data.GammaRecords)
    .FirstOrDefault();

if (gamma is not null)
{
    gamma.E = "123.45";
    gamma.DE = "6";
}

string output = string.Concat(
    datasets.Select(dataset => dataset.ToString()));

File.WriteAllText("edited.ens", output);
```

### Extract Gamma-Comment Values

```csharp
using System.Collections.Generic;
using ENSDF_Parser.Extractor;

if (datasets.Count > 0)
{
    var result = ValueExtractor.ParseFromComments(
        datasets[0],
        "E",
        new List<string> { "STOP" });

    ValueExtractor.RemoveInvalidEntries(
        result.Values,
        result.FoundErrors);

    foreach (var levelEntry in result.Values)
    {
        foreach (var gammaEntry in levelEntry.Value)
        {
            foreach (var value in gammaEntry.Value)
            {
                Console.WriteLine(
                    $"{value.Val} +/- {value.DVal}");
            }
        }
    }

    foreach (string message in result.FoundErrors)
    {
        Console.WriteLine(message);
    }
}
```

`RemoveInvalidEntries` is optional. It removes zero-uncertainty entries and appends diagnostic messages.

## Contributing

See [AGENTS.md](AGENTS.md) for repository conventions and contributor guidance.

When changing record handling, keep constructor offsets and serializer widths consistent. Verify representative input, dataset boundaries, record associations, and saved output relevant to the change.

Build the solution with:

```powershell
dotnet build ENSDF_Parser.slnx
```

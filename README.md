# ENSDF Parser

ENSDF-Parser is a C# library and interactive console application for working with Evaluated Nuclear Structure Data File (ENSDF) text. It reads fixed-column records into objects, organizes nuclear levels and radiation records, supports inspecting and editing record fields, and reconstructs dataset text. It also extracts numerical values and uncertainties from selected gamma comments.

The project is intended for programmatic inspection and small interactive data-processing workflows. It implements ENSDF-style record layouts; it is not a complete format validator or a lossless text editor. See [Known Limitations](#known-limitations) before relying on saved output.

## Requirements and Quick Start

Install the **.NET 10 SDK**. From the repository root:

```powershell
dotnet restore ENSDF_Parser.slnx
dotnet build ENSDF_Parser.slnx
dotnet run --project ENSDF_Parser_App/ENSDF_Parser_App.csproj
```

The last command opens an interactive prompt. Enter commands there, without a leading hyphen:

```text
parse file "C:\data\sample.ens"
dir
select dataset[0]
dir
get Header.IdentificationRecords[0].DSID
exit
```

Use your own input file; the repository does not include sample datasets or a test project. The example assumes at least one dataset with an identification record was loaded. Commands execute interactively; `Program` does not process command-line arguments as commands.

## Repository Structure

```text
ENSDF_Parser.slnx
ENSDF_Parser/                  Reusable .NET library
  Parser.cs                    Record recognition and dataset boundaries
  DataSet.cs                   Grouping and dataset serialization
  Records/                     Individual record models; Base/Record.cs
  MetaData/                    Identifier, RecordType, Header, Level, RadiationData
  Extractor/                   ValueExtractor and Value
ENSDF_Parser_App/              Console executable referencing the library
  Program.cs / UIApp.cs         Entry point and interactive loop
  InputParser.cs               Command dispatch
  Session.cs                   Loaded datasets, extraction results, file I/O
  Selection.cs / Tools.cs       Property navigation and command helpers
```

Both projects target `net10.0`. The console owns file loading and saving; the library accepts in-memory lines and exposes objects and text representations. Neither project declares external package dependencies.

## How Parsing Works

1. The console reads a file with `File.ReadAllLines` and calls `Parser.Parse(lines, fileLabel)`.
2. The parser reads the nuclide identifier from columns 1–5 and the four record-discriminator characters from columns 6–9.
3. Recognition rules select a record class. Its constructor slices the remaining fields at fixed offsets.
4. An empty or space-only line ends the accumulated dataset. A `DataSet` receives those records and the supplied file label.
5. `DataSet` groups header records, levels, radiation, and comments while retaining the original record sequence for output.

Radiation before the first level enters `UnplacedRecords`; subsequent radiation attaches to the current level. Comments are associated using their record marker and the most recently encountered matching target. Recognized continuation lines are represented as comments, rather than merged into typed fields.

## Object Model

| Type | Role |
| --- | --- |
| `DataSet` | Holds `Id`, a `File` label, `Header`, `Levels`, `UnplacedRecords`, and the private original record sequence. `GetOverview()` summarizes it. |
| `Header` | Collects identification, history, cross-reference, Q-value, and general comment records, plus normalization associations. |
| `Level` | Combines one `LevelRecord` with a `RadiationData` collection named `Data`. |
| `RadiationData` | Contains `GammaRecords`, `BettaRecords`, `ECRecords`, `AlphaRecords`, and `DelayedParticleRecords`. `BettaRecords` is the current API spelling. |
| `PNPair` | Groups `Parent`, `Normalization`, and `ProductionNormalization` records; members can be absent. |
| `Record` | Common base exposing `Id`, `RType`, and `Comments`, with text-formatting helpers. |
| `Identifier` | Stores the mass number in the byte field `Isotrope` and the remaining nuclide name in `Name`. |
| `RecordType` | Stores columns 6–9 as `C1`, `C2`, `C3`, and `C4`. |

For normalization grouping, a blank column-9 designator selects the latest parent; a nonblank designator searches preceding parents for a match. Production normalization uses the remembered normalization pair. `Header` also has collections for unpaired normalization and production-normalization records.

Most physical fields remain **strings**: for example, `GammaRecord.E`, `DE`, `RI`, and `M`. Setters trim leading and trailing whitespace but do not validate physical meaning or convert these fields to numbers.

## Supported Record Types

These are the recognition rules implemented by `Parser`; this table is not a complete ENSDF specification. Special comment, continuation, identification, normalization, and particle rules are evaluated before the ordinary column-8 switch.

| Marker | Class | Content |
| --- | --- | --- |
| Columns 6–9 blank, or a digit in 6 with 7–9 blank | `IdentificationRecord` | Dataset identifier, references, publication, date |
| `L` in column 8 | `LevelRecord` | Level energy, spin/parity, lifetime, and related fields |
| `G` | `GammaRecord` | Gamma energy, intensity, multipolarity, conversion data |
| `B` | `BetaRecord` | Beta energy, intensity, log ft |
| `E` | `ECRecord` | Electron-capture / positron fields |
| `A` | `AlphaRecord` | Alpha energy, intensity, hindrance factor |
| `P` | `ParentRecord` | Parent energy, spin/parity, lifetime, Q value |
| `N` | `NormalizationRecord` | Normalization factors and branching ratio |
| `PN` in columns 7–8 | `ProductionNormalizationRecord` | Combined normalization factors and options |
| `Q` in column 8 | `QValueRecord` | Q values, separation energies, references |
| `X` | `CrossReferenceRecord` | Dataset symbol and identifier |
| `H` | `HistoryRecord` | History text |
| Blank or `D` in 8; `N/P/A/D/T` in 9 | `DelayedParticleRecord` | Prompt or delayed particle fields |
| `C/D/T/c/t` in column 7 | `CommentRecord` | Comment text |

A continuation becomes `CommentRecord` when column 6 is neither blank nor `1`, columns 7 and 9 are blank, and column 8 is `L/B/E/G/H`. Unrecognized records fall back to the base `Record`. Blank dataset separators are consumed by the parser, not retained as dedicated objects.

## Fixed-Column Parsing and Serialization

Column numbers in this documentation are **one-based and inclusive**. C# slicing is zero-based: `Substring(9, 10)` reads columns 10–19. Do not split input records on whitespace or trim whole lines before parsing.

For a gamma record, representative fields are:

| Columns | Property |
| --- | --- |
| 1–5 | Nuclide identifier |
| 6–9 | Record discriminator |
| 10–19 / 20–21 | `E` / `DE` |
| 22–29 / 30–31 | `RI` / `DRI` |
| 32–41 | `M` |
| 42–49 / 50–55 | `MR` / `DMR` |
| 56–62 / 63–64 | `CC` / `DCC` |
| 65–74 / 75–76 | `TI` / `DTI` |
| 77 / 78 / 80 | `FLAG` / `COIN` / `Q` |

Each record class implements its own constructor slices and `ToString()` layout. `Record.FormatToNChars` right-pads short values with spaces and truncates overlong values. Reserved regions are reconstructed as spaces. Formatting therefore normalizes field whitespace rather than preserving the original line verbatim. These are implementation choices, not claims about required ENSDF alignment.

### Original order versus grouped objects

`DataSet.OrderedRecords` is a private list used by `DataSet.ToString()`. The header and level collections reference the same record instances:

- Changing an existing record's property through the grouped model changes that object's serialized text.
- Adding, removing, or replacing items in grouped collections does **not** synchronize the original sequence.
- Parsed comments appear in the original sequence as well as in their associated collections. Record serializers do not automatically append their `Comments` collection.

Saving a dataset serializes the original sequence, rather than traversing `Header` and `Levels`. See the output-related limitations below.

## Gamma-Comment Value Extraction

**GCV** stands for **Gamma Comment Values**. `ValueExtractor.ParseFromComments` visits gamma records under `DataSet.Levels` and processes comments with lowercase `c` in column 7. Results are grouped by level and gamma, alongside a list of diagnostic messages.

An identifier followed by `$` starts extraction. For example, with identifier `E`, a comment text fragment could be:

```text
E$ 12.34 {I5} 56.78 {I9} STOP
```

With `STOP` supplied as a breaker, the extractor reads two values. `Value.Val` contains the numeric value; `Value.DVal` stores absolute uncertainty scaled to the decimal places of the input. Thus `12.34 {I5}` gives `Val = 12.34` and `DVal = 0.05` under a culture accepting decimal points.

Extraction can continue across comments until a breaker or a different `$` marker is encountered. A plain numeric token after a value is accepted as uncertainty but logged as missing `{I...}` markup. Optional `r` filtering removes values whose uncertainty is missing or zero.

The export template is a quoted string. `val` is replaced with the value and `dval` with the uncertainty **digits** returned by `Value.GetString()`, not the absolute `DVal`. For the example above, `"val (dval)"` produces `12.34 (5)`. Replacements apply wherever those substrings occur.

Exports use this layout, where `File` is the input filename without its extension:

```text
<directory>/Values/Dataset-<File>/Level-<E>/Gamma-<E>.txt
<directory>/Logs/Dataset-<File>.txt
```

## Console Commands and Examples

Commands and property names are case-sensitive. Indices are zero-based; bracket lists use commas. Quote file paths and templates containing spaces. Angle-bracket placeholders below are explanatory and should not be typed literally.

| Command | Effect |
| --- | --- |
| `parse file "<file_path>"` | Loads and appends datasets to the session. |
| `parse gcv dataset[0,1] <identifier> [<breaker1>,<breaker2>] [r]` | Extracts gamma-comment values; trailing `r` is optional and removes zero/missing uncertainties. Use `[]` for no breakers. |
| `dir` | Lists the current object's properties, or the current list's count. |
| `select <property_path>` | Navigates to an object or indexed list item. |
| `select ..` | Moves back through the selection chain. |
| `get <property_path>` | Prints a property's value. |
| `set <property_path> <value>` | Assigns a value to a writable property; intended for string record fields. |
| `save dataset[0] "<file_path>"` | Serializes selected datasets to a file; comma-separated indices are accepted. |
| `save gcv "<directory_path>" "<template>"` | Exports all stored GCV results and diagnostics. |
| `exit` | Ends the session. |

In the GCV syntax, `[r]` denotes optional syntax: type the literal token `r`, without brackets, to enable it. Breaker brackets are literal.

Paths use dots between properties and brackets for list items. A path beginning with `dataset[0]` starts from the session's dataset list; other paths are relative to the current selection.

### Inspect and edit a gamma record

Assuming dataset 0 exposes at least one level containing a gamma:

```text
get dataset[0].Levels.Count
select dataset[0].Levels[0].Data.GammaRecords[0]
dir
get E
set E 123.45
get E
save dataset[0] "C:\data\edited.ens"
select ..
```

Save to a separate file and inspect the result before replacing source data. `save` overwrites its target. The console passes `set` values directly as strings; it does not convert arbitrary property types, and quoted values retain their quote characters.

### Extract and export comment values

For comments using the illustrative `E$` marker and `STOP` breaker above:

```text
parse gcv dataset[0] E [STOP] r
save gcv "C:\data\extracted" "val (dval)"
```

Use markers and breakers appropriate to your actual comments. Extraction stores a result per dataset; repeating extraction for the same dataset in one session currently raises a duplicate-key error.

## Using the Library

Reference `ENSDF_Parser/ENSDF_Parser.csproj` from your .NET project. The following example loads existing input and visits the levels exposed by the grouped model:

```csharp
using System;
using System.IO;
using System.Linq;
using ENSDF_Parser;

string path = "sample.ens";
var lines = File.ReadAllLines(path).ToList();
var datasets = Parser.Parse(lines, Path.GetFileNameWithoutExtension(path));

foreach (var dataset in datasets)
{
    Console.WriteLine(dataset.GetOverview());
    foreach (var level in dataset.Levels)
    {
        foreach (var gamma in level.Data.GammaRecords)
        {
            Console.WriteLine($"Level {level.LevelRecord.E}: gamma {gamma.E}");
        }
    }
}
```

`Parser.Parse` does not open the supplied filename; its second argument is a label stored on each dataset. `dataset.ToString()` returns reconstructed text. The boundary and grouping limitations below apply equally to library and console usage.

## Known Limitations

The following describe behavior directly visible in the current source. They are **implementation limitations, not ENSDF specification requirements**.

- **Final dataset handling:** [`Parser.Parse`](ENSDF_Parser/Parser.cs) emits datasets only upon empty or space-only lines and does not flush remaining records at EOF. A record list without a final blank separator yields no final dataset. A line terminator alone is not an additional blank line. Tabs alone are not recognized as separators by this check.
- **Final level handling:** [`DataSet.DistributeRecords`](ENSDF_Parser/DataSet.cs) adds the previous level only when the next level arrives. A dataset with one level exposes an empty `Levels` list; the final level and its radiation remain in the original sequence but are absent from grouped traversal and GCV extraction.
- **Input validation:** Constructors use direct substring offsets without padding short lines. Many need 80 characters and throw on shorter input. Identifier mass numbers are parsed as `byte`, so values above 255 fail. There is no per-line diagnostic wrapper or verification that a dataset starts with an identification record.
- **Identifier formatting:** [`Identifier.ToString()`](ENSDF_Parser/MetaData/Identifier.cs) omits five-column padding. For example, input identifier `" 60CO"` becomes `"60CO"`, shifting subsequent fields in ordinary record serializers. Cross-reference and particle serializers also assume a correctly sized prefix when taking its first eight characters.
- **Dataset output boundaries:** `DataSet.ToString()` joins records with LF and ends with LF plus 85 spaces, without a following newline. [`Session.SaveDatasets`](ENSDF_Parser_App/Session.cs) concatenates datasets directly, so saving multiple datasets places the next first record on the same line as those spaces.
- **Text preservation:** Unknown record payloads are discarded by the base `Record`. Field trimming, truncation, and regenerated reserved spaces prevent byte-for-byte round trips. Grouped collection edits do not update `OrderedRecords`, as explained above.
- **Normalization and comments:** Unmatched nonblank normalization designators are not added to the unpaired collection. The remembered normalization pair is not reset by intervening unrelated records. Normalization comments target the latest parent pair, even if a designator selected another pair, and can dereference an absent normalization. Some comments without matching targets are omitted from grouped collections while remaining in the original sequence.
- **Extraction scope and numbers:** [`ValueExtractor`](ENSDF_Parser/Extractor/ValueExtractor.cs) excludes unplaced gamma records and non-lowercase-`c` comments. Its extraction state is not reset between gamma records or levels. Numeric parsing uses the current culture; [`Value`](ENSDF_Parser/Extractor/Value.cs) determines precision by counting characters after a decimal point, without interpreting exponent notation.
- **Console navigation and export:** Reflection navigation accesses properties, not public fields such as the members of `PNPair`. GCV output names use file labels and energy strings without unique IDs or filename sanitization; repeated names can overwrite earlier exports.

# ENSDF Parser

This is a simple ENSDF parser, supported with Console application for easier use.
The Parser is following the official file format

This is a list of the commands for the application:

-parse file {file_path}

-parse gcv {dataset[indices]} {identifier} {breakers}

-get {property_path}

-select {property_path}

-set {property_path} {value}

-save {dataset[indices]} {file_path}

-save gcv {directory_path} {"template"}

GCV stands for Gamma Comment Values and has type of Value (contains Value and Uncertainty)
The template is a quoted string, in which the word "val" is replaced with the actual value and "dval" is replaced with the uncertainty

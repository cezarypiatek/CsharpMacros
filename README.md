# CsharpMacros
A simple template base system of macros for Visual Studio that can be executed in design time

![examplle usage](doc/Macro.gif)

## Releases
- [nuget](https://www.nuget.org/packages/CsharpMacros/)
- [VS extension](https://github.com/cezarypiatek/CsharpMacros/releases)

### Support for VisualStudio.
Install as a nuget package or vsix. Verify your Roslyn integration option in case you are using R#.

### Support JetBrains Rider
Install as a nuget package

### Support for VSCode
Install as a nuget package and check `Enable support for roslyn analyzers, code fixes and rulesets` in Settings.


## Macro anatomy

![macro anatomy](/doc/macro_anatomy.jpg)

Every macro consists of the following parts

- `Macro Header` in the following format `macro(varname in macro_name(macro_params))` where `varname` represents variable that holds the reference to every element returned by the macro. `macro_name` represents one of the predefined functions that returns data for the template. `macro_params` is the input for macro function.

- `Macro Template` - a template of code that will be repeated for every element returned by the macro function. The template can contain placeholders in the following format `${varname.attribute_name}` that will be replaced with a given attribute value of the element returned by the macro function.

## Currently available macro functions

- `properties` - return a list of properties of given type accepted as the parameter. Available attributes: 
    - `name` - name of the property
    - `type` - type of the property
# bbaToolS5

Full rewrite of yoqs old bbaTool (my fork: https://github.com/mcb5637/bbaTool, original repo got deleted).
Allows to read and write Settlers 5 HoK archive files (.bba / .s5x).

Simple Usage: Just drop a file or folder onto the executable to unpack or pack it.

Advanced usage:
- Command line option `-err` for use in scripts, disables waiting for input before closing and exits with an error code on errors.
- All other parameters are used as paths:
  - if only one file is specified, the output path is determined automatically.
  - else, the last path is used as output (archive or folder determined by extension).
  - all other paths are loaded in order (possibly overriding contents) as input (archive or folder determined by checking for existing file/directory).

# S5XTools

GUI Tool to view and modify Settlers 5 HoK archive files (.bba / .s5x).
Also includes automated common tasks like script processing, preview image replacement and folder to s5x map conversion.
Note: Currently you cannot override a file that is still loaded.


Included lua tools:
- http://www.lua.org/
- http://luabinaries.sourceforge.net/

# bbaToolS5

Full rewrite of yoqs old bbaTool (my fork: https://github.com/mcb5637/bbaTool, original repo got deleted).
Allows to read and write Settlers 5 HoK archive files (.bba / .s5x).

Simple Usage: Just drop a file or folder onto the executable to unpack or pack it.
If a target file/folder exists, asks if it should be remove (override), added (still overrides existing files, if they exist in both) or cancel.

Advanced usage:
- Command line options:
	- `-err` for use in scripts, disables waiting for input before closing and exits with an error code on errors.
	- `-ignorehidden` when reading a folder, ignore all files/folders that are hidden (usefull to pack a git repository).
	- `-autoCompression` automatically choose if a file should be compressed or not, based on size (never compresses audio files, this would prevent them from being played by SHoK).
	- `-searchDuplicates` directly before storing an archive, search for duplicate files and link them.
	- `-override` force override target files/folders.
	- `-add` automatically add to target file/folders.
- All other parameters are used as paths:
	- if only one file is specified, the output path is determined automatically.
	- else, the last path is used as output (archive or folder determined by extension).
	- all other paths are loaded in order (possibly overriding contents) as input (archive or folder determined by checking for existing file/directory).

# Linked files
When writing a bba/s5x archive, the bbaTool can link files, pointing multiple filename entries at the same archived file.
Can reduce archive size, especially if used with textures.
(For example, each s5x contains 3 times the same `graphics\\textures\\gui\\mappics\\externalmap.png`.)

# S5XTools

GUI Tool to view and modify Settlers 5 HoK archive files (.bba / .s5x).
Also includes automated common tasks like script processing, preview image replacement and folder to s5x map conversion.

Default way to load mcbPacker to your mapscript:
```lua
Script.Load("data\\maps\\user\\s5CommunityLib\\packer\\devLoad.lua") --mcbPacker.ignore
mcbPacker.Paths = {{"data/maps/user/EMS/tools/", ".lua"}} --mcbPacker.ignore
--mcbPacker.addLoader
```

Included lua tools:
- http://www.lua.org/
- http://luabinaries.sourceforge.net/

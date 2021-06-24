
--- represents a S5 archive (bba or s5x),
--- @class ArchiveAccess
ArchiveAccess = {}

--- loads an existing archive on top of everything currently loaded.
--- @param path string path to the archive
function ArchiveAccess:LoadBba(path) end

--- loads a folder on top of everything currently loaded.
--- @param path string path to the folder
function ArchiveAccess:LoadFolder(path) end

--- writes the archive to a bba (or s5x).
--- you cannot override an archive that is currenly opened.
--- @param path string path to the archive
function ArchiveAccess:WriteBba(path) end

--- writes the archive to a folder.
--- @param path string path to the folder
function ArchiveAccess:WriteFolder(path) end

--- adds a file from disk.
--- @param file string file to add
--- @param internal string name for the file inside the archive
function ArchiveAccess:AddFileFromFilesystem(file, internal) end

--- adds a file from memory.
--- @param data string string to add as a file
--- @param internal string name for the file inside the archive
function ArchiveAccess:AddFileFromString(data, internal) end

--- gets a single file from the archive as string.
--- throws if file doesnt exist.
--- @param internal string name inside the archive
--- @return string data file as a string
function ArchiveAccess:GetFile(internal) end

--- removes a file from the archive.
--- throws if file doesnt exist.
--- @param internal string name inside the archive
function ArchiveAccess:RemoveFile(internal) end

--- renames a file inside the archive.
--- throws if file doesnt exist.
--- @param oldname string current name
--- @param newname string new name
function ArchiveAccess:RenameFile(oldname, newname) end

--- gets all files currently in the archive.
--- @return string[]
function ArchiveAccess:GetFileNames() end

--- removes everything from the archive.
--- releases resources used immediately, instead of waiting for the file to be GCed.
function ArchiveAccess:Clear() end

--- creates a new archive object.
--- @return ArchiveAccess
function NewArchive() end

--- sets an archive to be shown in the UI and refreshes the UI. clears the old one, if it is not the same as the new one.
---@param archive ArchiveAccess new archive
function SetArchiveForUI(archive) end

--- gets the archive currently shown in the UI.
--- @return ArchiveAccess
function GetArchiveForUI() end

--- generates a GUID.
--- @return string GUID
--- @see MapFileSetGUID to set the GUID.
function GenerateGUID() end

--- sets a mapfiles GUID. Throws if no info.xml can be found.
--- @param archive ArchiveAccess mapfile
--- @param GUID string new guid
--- @see GenerateGUID to generate a new random GUID.
function MapFileSetGUID(archive, GUID) end

--- gets the mapname and description from a maps info.xml.
--- @param archive ArchiveAccess mapfile
--- @return string|nil name mapname, or nil of no info.xml is found
--- @return string|nil desc description, or nil of no info.xml is found
function MapFileGetNameAndText(archive) end

--- gets the mapname and description from a maps info.xml.
--- throws if no info.xml is found.
--- @param archive ArchiveAccess mapfile
--- @param name string mapname
--- @param desc string description
function MapFileSetNameAndText(archive, name, desc) end

--- @class Path
Path = {}
--- path to search for
Path.Path=""
--- if the path is part of the current archive
Path.InArchive=false

--- processes a script, finding any requirements and adding them to the archive.
--- @param archive ArchiveAccess mapfile
--- @param outname string output file, ex. "mapscript" for the main map script
--- @param inname string input file, require name or full path to an external file
--- @param paths Path[] search path, first entry gets searched first
--- @param copyToOne boolean set to true, if all files should be copied together to one file
--- @param addloader boolean set to true, if you want to add a stub mapscript that hides the main script (only does something if outname is "mapscript")
--- @param compile boolean set to true, if you want to precompile the resulting scripts (requires addloader)
--- @return string log a log with any errors encountered
function PackLuaScript(archive, outname, inname, paths, copyToOne, addloader, compile) end

--- set to the game path, if it could be found in the registry.
--- @type string|nil
S5InstallPath = nil

--- set to the file path of the currently executing lua makro.
--- @type string
MakroFile = nil

--- current archive externalmap folder for use in PackLuaScript.
--- @type Path
ExternalmapPath = nil

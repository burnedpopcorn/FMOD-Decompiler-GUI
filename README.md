# FMOD-Decompiler-GUI

FMOD-Decompiler-GUI is a tool that attempts to decompile multiple FMOD bank files into a single `.fspro` project. This allows users to reconstruct an FMOD project from compiled bank files, making it easier to analyze or modify existing audio assets.<br>


This uses DLLs from the [FMOD Studio/Core API](https://fmod.com/download#fmodengine).

## Features
- Parses and extracts data from FMOD bank files (`.bank`)
- Attempts to reconstruct an FMOD Studio Project from the bank files (`.fspro`)
- Supports multiple bank files for a unified project
- Uses AvaloniaUI for its GUI
- Uses [FMOD-Decompiler CMD](https://github.com/doggywatty/FMOD-Decompiler) as a Backend, which can be upgraded without needing to upgrade the GUI build

## Limitations
- This tool attempts to reconstruct `.fspro` projects and may not be fully accurate.
- This tool only supports FSB5 Bank files
- Some metadata or complex FMOD features may not be fully recovered.
- Compatibility with newer versions of FMOD Studio is not guaranteed.

## Contributing
Pull requests and issue reports are welcome! Feel free to contribute improvements or report bugs.

## License
This project is licensed under the GNU License. See `LICENSE` for more details.

## Credits
- The Original Console App [FMOD-Decompiler](https://github.com/doggywatty/FMOD-Decompiler)
- [FenixProFmod](https://github.com/M0n7y5/FenixProFmod), used as a template for the UI

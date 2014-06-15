This repository holds sources of Quanti workflow - software for label-free quantitative proteomics analysis by mass spectrometry data. 
To get Installation package for Windows (32/64-bit), additional project documentation and test files, please, access the project page http://pkki.mbb.ki.se/Quanti/
The ready-to-build for Win64 solution for Visual Studio 2010 located in Quanti Workflow subfolder.
To build 32-bit applications you have to change in appropriate binary folders 64-bit files of SQLite to 32-bit ones provided in /SQLite32 folder.
Also for access to Thermo .raw files you have install MSFileReader and to register (regsvr32 [DllFileName]) XRawFile2.dll for 32-bit Windows or XRawFile2_x64.dll for 64-bit Windows.

For citation, please use:
In silico instrumental response correction improves precision of label-free proteomics and accuracy of proteomics-based predictive models.
Lyutvinskiy Y, Yang H, Rutishauser D, Zubarev RA. Mol Cell Proteomics. 2013 Aug;12(8):2324-31. doi: 10.1074/mcp.O112.023804. Epub 2013 Apr 15.

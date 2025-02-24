JUST OPEN WITH ADMINISTRATOR RIGHTS ocxInit.bat<br>
OR REGISTER FPCLOCK_Svr.ocx AND FP_CLOCK.ocx <br>
VIA OPENING cmd WITH ADMINISTRATOR RIGHTS AND DO:
```
regsvr32 FPCLOCK_Svr.ocx
regsvr32 FP_CLOCK.ocx
```

!!! TMPCCOMM.dll IS NEED TO BE IN ONE FOLDER WITH FP_CLOCK.ocx !!!

NEXT GO TO <br>
|> *.cs[Design] # ANY FORM DESIGNER<br>
|> Toolbox <br>
|> RIGHT MOUSE CLICK <br>
|> Choose items... <br>
|> COM Components<br>
|> Browse...<br>
|> SELECT FPCLOCK_Svr.ocx<br>

NEXT NEED ALL OTHER DLLS SUCH AS:
- Interop.FP_CLOCKLib.dll
- Interop.FPCLOCK_SVRLib.dll
- AxInterop.FP_CLOCKLib.dll
- AxInterop.FPCLOCK_SVRLib.dll

TO BE IN THE APP TARGET DIRECTORY
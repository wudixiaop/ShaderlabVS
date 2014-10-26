ShaderlabVS
===========

ShaderlabVS is a Visual Studio Plugin for Unity3D shader programming.  

####Screenshot Preview

* **Syntax Hightlighting and outlining**
	![](./img/Highlighting.PNG)
* **Quickinfo**
	![](./img/QuickInfo.PNG)
* **Code Completion**
	![](./img/CodeCompletion.PNG)
* **Signature help for CG and Unity3d built in functions**
	![](./img/SignatureHelp.PNG)

####Features Overiew:
* Outlining
* Quickinfo
* Code Completion
* Comment/UnComment shorcuts
    * Press "__Ctrl + E, C__" to comment
    * Press "__Ctrl + E, U__" to uncomment
* Signature help for CG and Unity3d built in functions
* Syntax Hightlighting
    * support .shader, .compute, .cginc and .glslinc files 
    * Keywords are case insensitive


####How to build and debug:
1. Download and install Visual Studio SDK
2. Open ShaderlabVS solution
3. Press *F6* to build solution
4. If you want to debug it in Visual Studio, and encounter problems, please make sure the **_Start exteral program_** and **_Comand line arguments_** in the **Debug** tab of ShaderlabVS project settings have value as follow:
    1. Set **_Start exteral program_** to the path of devenv.exe (the Visual studio main program)
    2. set **_Comand line arguments_** to **/rootsuffix Exp**. Below is a screenshot for the settings:
    ![](./img/DebugSettings.PNG)

####Support Visual Studio Version:
* Visual Studio 2012
* Visual Studio 2013
* The other version are not tested, not sure if there are also work or not.

####License
MIT



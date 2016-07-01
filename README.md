ShaderlabVS
===========

ShaderlabVS is a Visual Studio Plugin for Unity Shaderlab programming. It supports support .shader, .compute, .cginc and .glslinc files.  Latest releae build can be found at [here](http://blog.shuiguzi.com/2014/10/28/Release/).

[![Open Source Love](https://badges.frapsoft.com/os/mit/mit.svg?v=102)](https://github.com/wudixiaop/ShaderlabVS/) [![GitHub version](https://d25lcipzij17d.cloudfront.net/badge.svg?id=gh&type=6&v=0.6.1&x2=0)](http://blog.shuiguzi.com/2014/10/28/Release/)

###Features

######1. Syntax Hightlighting and outlining

![Highlighting](./img/Highlighting.PNG)
######2. Quickinfo

![QuickInfo](./img/QuickInfo.PNG)

######3. Code Completion

![CodeCompletion](./img/CodeCompletion.PNG)

######4. Signature help for CG and Unity built in functions

![SignatureHelp](./img/SignatureHelp.PNG)

###Development
#####1. Requirements 

* Visual Studio
* Visual Studio SDK

#####2. How to debug in Visual Studio
1. Download and install Visual Studio SDK
2. Open ShaderlabVS solution
3. Press *F6* to build solution
4. If you want to debug it in Visual Studio, and encounter problems, please make sure the **_Start exteral program_** and **_Comand line arguments_** in the **Debug** tab of ShaderlabVS project settings have value as follow:
    1. Set **_Start exteral program_** to the path of devenv.exe (the Visual studio main program)
    2. set **_Comand line arguments_** to **/rootsuffix Exp**. Below is a screenshot for the settings:
    ![](./img/DebugSettings.PNG)

###Support Visual Studio Versions:
* Visual Studio 2012
* Visual Studio 2013
* Visual Studio 2015 (Need execute vs2015_register.cmd after the plugin install)

__The other version are not tested, not sure if there are also work or not.__


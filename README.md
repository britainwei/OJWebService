**This plugin is in BETA version and NOT recommended to use in production sites**

Introduction
============
This project is a webservice project for OnlineJudge of HIT, written in C#. The main function of this project is faultloction, using Yang Shaojun's Fault Location Algorithm. Please do not reproduced, distribute or quote without written permission of the author.

This project is base on Windows7 platform, using Visual Studio 2013. The first author is Yang Shaojun, and secondary developed by britain(Email: britainwei@163.com). 

Detail
======
You can follow this project and than import in visual studio 2013

Then deployed the generate website on IIS.

Finally you can visit it in url.

FAQ:
1. System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)

Maybe the IO stream handler is not release before when you need it. Make sure all the IO stream is close after using it.

2. The IIS is not allowed to show the derectory.

In IIS's content view, open the Directory Browse Mode

3. Can't find the ISAPI And CGI Limit in IIS's content view.

The IIS was not install all modules. You need to install them as follows:
~~~~~
control panel > Program > open or close Windows Features
~~~~~
You need to select all the checkbox in IIS Manager... under the  Internet Information Service, one by one.
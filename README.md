**This plugin is in BETA version and NOT recommended to use in production sites**

Introduction
============
此工程用于为乐学网代码自动评测系统提供错误定位服务。错误定位算法采用杨劭君的《基于加权行为图挖掘的错误定位算法研究》，核心算法都是他实现的，本人只是基于他的算法进行改装，封装成webservice服务，为乐学网提供代码逻辑错误定位服务。未经允许不得用于商业服务。
E-mail : britainwei@163.com

Environment
===========
此项目在windows7下采用vs2013开发，发布部署在IIS上即可使用

FAQ
===
1. System.IO.__Error.WinIOError(Int32 errorCode, String maybeFullPath)
----------------------------------------------------------------------
Maybe the IO stream handler is not release before when you need it. Make sure all the IO stream is close after using it.

2. The IIS is not allowed to show the derectory.
-------------------------------------------------
In IIS's content view, open the Directory Browse Mode

3. Can't find the ISAPI And CGI Limit in IIS's content view.
-------------------------------------------------------------
The IIS was not install all modules. You need to install them as follows:
~~~~~
control panel > Program > open or close Windows Features
~~~~~
You need to select all the checkbox in IIS Manager... under the  Internet Information Service, one by one.
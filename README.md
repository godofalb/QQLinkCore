# QQLinkCore
== finallres中的文件为机器人程序，pluginDir中有2个现成插件（并非插件示例）。
== QQLinkCore2中是与qq接入部分源码
== WindowsV1中是用户界面部分源码
== PlugInDemo部分是插件框架

运行顺序：根据PlugInDemo编写插件，在res中找到插件对应.dll和配置文件，将其拷贝到finallres\pluginDir下，启动finallres\WindowsV1即可。
其余部分请仔细阅读PlugInDemo的注释和配置文件。配置文件包括与插件同名的配置文件（在PlugInDemo\Res下，需要一同拷贝）和WindowsV1.exe配置文件(WindowsV1.exe.config)
尤其是后者。因为这个机器人通过不断向某个讨论组发送消息保持登入状态，所以需要配置一个讨论组用来接收该信息。

因为人员不足，本软件已经基本停止更新，有任何使用问题请联系作者本人。QQ：3220665887
虽然由我说不太好，这个软件是很方便使用的。虽然看起来很复杂。

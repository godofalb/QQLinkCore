   M S B A M L   `   `   `  ���� D  @WindowsV1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:WindowsV1	WindowsV1      WindowsV1.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     )localclr-namespace:WindowsV1     -     � Title$  QQLInker VB1.23��$	��360��$	��525�� � 	Resources    listBoxStyle �� 
TargetType �� Property �� Value �� DataType   WindowsV1.PlugInP �� Orientation � DataContext �� Mode  ContextMenu1��%�  &	        &	 �    �� " ��.���  � �� �� "  .R��� -   $ 
Horizontal=� �� .+��� ���� *Name+�� $��
使用中:���� ���� *Used+�� $��使用好友信息:���� ���� *NeedF+$ OneWay=��� $��使用群信息:���� ���� *NeedG+$ OneWay=��� $��使用讨论组信息:���� ���� *NeedD+$ OneWay=��� -   
�������$��修改��$��0q� �� *Class+�� .@�m� $��refresh��-   $��刷新��.��� 	 � ColumnDefinitions	 �� �� .+��� .+��� -   $��	loginbox1��$��0��$
��Left=�$	��290��$	��239��$	��Top=�$��	10,10,0,0q��� -   $	��Sig��$��需要登入��$��0q�$��Center=�� -   $��commands��$��1��
 � RowDefinitions
 �� $	��10* ��� $	��70* ��� $	��15* ��� $	��15* �.+��� -   $��DirPanel��$��0��$ 
Horizontal=�.+��� -	   $��Label1��$��插件目录:��$��	10,0,10,0q��� -
   $	��dir��$��Center=�$
��Left=�$��0q�$	��110��$�� ���� -   $��	ChangeDir��
�������$��0q�$��Right=�$��50��$��Stretch=�$��5q�$��改变目录���� -   $��plugins�� � ContextMenu# [ $��1��$��0,5.2,0.2,0q�#��[  ���� $ OneWay=��� -   $��unload��$��2��$	��100��$��卸载插件��$��10,10,10,10q�$
��Left=��� -   $��reload��$��2��$	��100��$��重载插件��$��10,10,10,10q�$��Right=��� -   $��do��$��3��$��开始使用��$��10,10,10,10q�
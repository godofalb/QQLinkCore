   M S B A M L   `   `   `  ���� D  @WindowsV1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null%clr-namespace:WindowsV1	WindowsV1      WindowsV1.Edit   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     )localclr-namespace:WindowsV1     -     � Title$
  Edit��$	��400��$	��400�� � 	Resources    listBoxStyle �� 
TargetType �� Property �� Value �� DataType   WindowsV1.Pair �� Orientation �� Mode��%�   &	        �� " ��.���  � �� �� "  .R��� $ 
Horizontal=�.+��� ���� *Key+�� .��:�� ���� *Value+$ TwoWay=�.���  � RowDefinitions �� $	��30* ��� $	��40* ��� $	��40* ��� $	��40* ��� $	��20* �.+��� -   $��BaseData��$��Center=�$��0��$ 
Horizontal=�$��2q�.+��� $��Center=�$��5q�	 d� 	IsChecked	 �� 
 �� Path$

 Used�$ TwoWay=�.��	使用中�� $��Center=�$��5q�	 �� $
 NeedF�$ TwoWay=�{��� $
 FChangeable�.��获取好友信息�� $��Center=�$��5q�	 �� $
 NeedG�$ TwoWay=�{��� $
 GChangeable�.��获取群信息�� $��Center=�$��5q�	 �� $
 NeedD�$ TwoWay=�{��� $
 DChangeable�.��获得讨论组信息� -   $��	BaseData2��$��Center=�$��1��$	��350�� �� �� ��  � ColumnDefinitions �� $	��30* ��� $	��30* ��� $	��30* �.+��� $��0��$��0��.��好友信息模式�� $��0��$��1��.��群信息模式�� $��0��$��2��.��组信息模式�� $��1��$��0������ $
 Friends�$ TwoWay=��� $��1��$��1������ $
 Groups�$ TwoWay=��� $��1��$��2������ $	
 Dis�$ TwoWay=��� $��2��$��0������ $
 
FriendMode�$ TwoWay=��� $��2��$��1������ $
 	GroupMode�$ TwoWay=��� $��2��$��2������ $
 DisMode�$ TwoWay=��� -   $��DataList��$��2��#��[  ���� �� -   $	��Des��$��3�� �� AutoWordSelection . �� -   $��
SaveButton��$��4��$��Right=�$	��100��$��10q�$��保存配置��
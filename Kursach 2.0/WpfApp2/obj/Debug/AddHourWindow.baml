   M S B A M L   `   `   `  ����B  >WpfApp2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null!clr-namespace:WpfApp2WpfApp2  5   	       WpfApp2.AddHourWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   'localclr-namespace:WpfApp2     5   	     � Title$  AddHourWindow��5   	   $	��323��6   $	��400��6,    � 
ResizeMode$ NoResize=�68   .��5	      � $��0,0,0,-6q�6   .+�5
   
   �� -   5        $��Добавить час��5
      $��Center=�6(   $��	0,200,0,0q�6E   $	��Top=�6X   $��Arial)�6p   $��16(�6�   $	��212��6�    J� RenderTransformOrigin$ 0.511,1.363&�6�   $��31��6�   
���   ����	#FF4D4D4D
�������6  (  "System.Windows.Controls.DatePicker  5   
   -   5        $��
datePicker��5      $��Center=�6'   $��	0,124,0,0q�6D   $	��Top=�6W   $��Arial)�6o   $��16(�6�   $	��212��6�   
���   ����	#FF4D4D4D6�   �� 5   
   -   5        $��DateBox��5   0   $��Center=�6?    �� TextWrapping$
 Wrap=�6\   $%��Количество часов��6p   $	��Top=�6�   $	��212��6�   $��Arial)�6�   $��16(�6�   $��24��6�   $��	0,164,0,0q�6�   
���   �6�   �� 5   
   -   5        $��NameBox��5   %    �� 
IsEditable . $��Center=�64   $��0,48,0,0q�6Q   $	��Top=�6c   $	��212��6{   $��29��6�   
���   ���5      �� 5       �� EndPoint$ 0.5,1&�6&    �� 
StartPoint$ 0.5,0&�67   .��5      �  � Color$ Black��6#   � 5      $ 	#FFE5E5E5��6#    � Offset$ 1Q�65   5      5      �� 5   
   -   5        $��GroupBox��5   %    . $��Center=�65   $��0,86,0,0q�6R   $	��Top=�6d   $	��212��6|   $��29��6�   
���   �6�   �� 5   
   $��Предмет:��6   $
��Left=�6#   $��	28,48,0,0q�6>   $	��Top=�6Q   �� 5   
   $��Группа:��6   $
��Left=�6"   $��	35,86,0,0q�6=   $	��Top=�6P   5      5      
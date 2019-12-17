一、CustomEntity是自定义实体
1 Tunnel是巷道
2 TunnelNode是巷道的节点
3 Tag是巷道的标注
二、EntityManagment是对自定义实体的cli封装
三、EntiyStore是数据库liteDB的有关的类，包括储存模型与数据库操作
四、UI包括了与CAD对接的接口，鼠标动态生成巷道在（DynamicCreate.cs），各种其他命令在（SampleFunc.cs）
五、TestProgram是调试程序用的
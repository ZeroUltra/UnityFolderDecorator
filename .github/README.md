## Unity FolderDecorator

用于标记Unity文件夹

##### 小视图

 ![image-20250820141235102](C:\Users\y\AppData\Roaming\Typora\typora-user-images\image-20250820141235102.png)

![image-20250820133832694](https://raw.githubusercontent.com/ZeroUltra/MediaLibrary/main/Imgs/202508201338136.png)

##### 大视图

![image-20250820134220040](https://raw.githubusercontent.com/ZeroUltra/MediaLibrary/main/Imgs/202508201342223.png)

## 使用步骤

* 直接下载压缩包导入 或者使用packagemanager add github url
* 然后鼠标右键Create->FolderDecorateSetting 创建配置文件
* 进行目录配置

## 参数说明

* **Folder**  目标文件夹 (直接拖入unity文件夹)
* **BuiltinIconName** 内置图标名字 (可以查看 [halak/unity-editor-icons](https://github.com/halak/unity-editor-icons)   或者 [jasursadikov/unity-editor-icons: ✨ Gallery of Unity Editor icons (6000.0.7f1)](https://github.com/jasursadikov/unity-editor-icons))
* **CustomIcon** 指定一个图片资源
* **IconOffset** 图标偏移
* **LabelStyle** 文本风格
* **LabelColor** 文本颜色(如果透明度为0,使用默认颜色)
* **BackgroundColor** 背景颜色
* **BacgroundRadius** 边框圆角 (大视图会忽略)  查看:[Unity - Scripting API: GUI.DrawTexture](https://docs.unity3d.com/2022.3/Documentation/ScriptReference/GUI.DrawTexture.html)
* **BackgroundBorderWidth** 边框的宽度 (大视图会忽略)。如果为0，则绘制完整的纹理 查看:[Unity - Scripting API: GUI.DrawTexture](https://docs.unity3d.com/2022.3/Documentation/ScriptReference/GUI.DrawTexture.html)
* **Tooltip** 提示

**注:IconName 和 CustomIcon 只会显示一个 优先显示IconName** 



最后如果没有应用设置 点一下Apply按钮

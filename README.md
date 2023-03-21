# AppList
一个列表程序。

无论有没有管理员权限，点击安装后都是以runas /env启动文件。

需配合[Tamamopoi/MyRunas](https://github.com/Tamamopoi/MyRunas)实现。

只能用于AD域环境安装程序。大概。

## 前置要求
- 配置myrunas

- 任意php环境

## 关于AppList
- 读取共享目录（由PHP显示目录内所有文件），自动绘制控件。

- 显示用户名及本地IP

- 点击安装按钮，执行myrunas提权安装

## 使用方式
修改`string url = "http://172.31.218.53/";`为实际PHP地址

修改`string command = string.Format(@"myrunas /env /user:aDMh7PBKZWnBbpuiWds3DQ--@microsoft.com ""\\ADSERVER\app\{0}""", fileName);`为实际命令与实际共享路径

重新运行，去debug目录复制exe程序即可使用。

## 显示目录下所有EXE后缀的PHP代码

```
<?php
$path = 'C:\share\app';
$files = glob($path . '/*.exe');
usort($files, function($a, $b) {
    return strcmp(basename($a), basename($b));
});
foreach ($files as $file) {
    echo basename($file) . '|' . date('Y-m-d', filemtime($file)) . '<br>';
}
?>
```

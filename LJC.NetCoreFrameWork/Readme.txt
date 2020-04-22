##ImageHelper 在linux系统上需要作一些额外的事情

#ImageHelper需要安装库
yum update && \
yum install libgdiplus libc6-dev -y && \
ln -s /usr/lib/libgdiplus.so /usr/lib/gdiplus.dll && \
ln -s /usr/lib/x86_64-linux-gnu/libdl.so /usr/lib/libdl.dll

#安装字体,否则乱码
mkdir -pv /usr/share/fonts/chinese/TrueType
cp font/* /usr/share/fonts/chinese/TrueType/ #把你的字体复制到Linux上
chmod 755 /usr/share/fonts/chinese/TrueType
#建立字体缓存
 mkfontscale #（如果提示 mkfontscale: command not found，需自行安装 # yum install mkfontscale ）
 mkfontdir #
 fc-cache -fv #（如果提示 fc-cache: command not found，则需要安装# yum install fontconfig ）

##
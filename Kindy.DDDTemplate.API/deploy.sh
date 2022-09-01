#! /bin/sh

#接收外部参数
harbor_url=$1
harbor_project_name=$2
project_name=$3
tag=$4
port=$5
env=$6

imageName=$harbor_url/$harbor_project_name/$project_name:$tag
echo "$imageName"
 
#查询容器是否存在，存在则删除
containerId=`docker ps -a | grep -w $project_name:$tag  | awk '{print $1}'`
if [ "$containerId" !=  "" ] ; then
    docker stop $containerId
    docker rm $containerId
    echo "delteted docker $containerId"
fi
 
#查询镜像是否存在，存在则删除
imageId=`docker images | grep -w $project_name  | awk '{print $3}'`
if [ "$imageId" !=  "" ] ; then
    docker rmi -f $imageId
    echo "deleted image $imageId"
fi

# 登录Harbor
#docker login -u harborurl -p root root
 
# 下载镜像
docker pull $imageName 

# 启动容器
docker run -d -p $port:80 --name $project_name -e ASPNETCORE_ENVIRONMENT=$env $imageName



#!/usr/bin/env groovy
pipeline {
    agent { label 'node1' }

    environment {
        git_credentialsId = 'd0ad638a-0a98-415e-824f-02cb14f8a6fd' // git凭证
    }

    parameters {
        string(name: 'ASPNETCORE_ENVIRONMENT', defaultValue: 'Development', description: '项目名称')
        string(name: 'PROJECT_NAME', defaultValue: 'kindy', description: '项目名称')
        string(name: 'PROJECT_FILEPATH', defaultValue: './Kindy.DDDTemplate.API/Kindy.DDDTemplate.API.csproj', description: '源代码编译地址')
        string(name: 'HOST_PORT', defaultValue: '8188', description: '主机端口')
        string(name: 'HARBOR_URL', defaultValue: 'tangzixuan', description: 'harbor地址')
        string(name: 'HARBOR_GROUP', defaultValue: '', description: 'harbor工作目录')
        string(name: 'HARBOR_USER', defaultValue: '', description: 'harbor用户')
        string(name: 'HARBOR_PWD', defaultValue: '', description: 'harbor密码')
        string(name: 'DOCKERFILE', defaultValue: './Kindy.DDDTemplate.API/Dockerfile', description: 'dockerfile地址')
        string(name: 'GIT_URL', defaultValue: 'https://github.com/kaibbo/Kandy.git', description: 'giturl地址')
        string(name: 'Branch', defaultValue: 'master', description: 'git branch')
        string(name: 'CALLBACK_URL', defaultValue: '', description: 'callback url')
    }

    stages {
        stage('Pull Code') {
            steps {
                script {
                    /* 清空项目目录缓存 */
                    sh 'rm -rf ./**'
                    echo '======================================git clone begin=============================================='
                    /* 拉取项目代码*/
                    git branch: "${params.Branch}", credentialsId: "${git_credentialsId}", url: "${params.GIT_URL}"
                    echo '======================================git clone end=============================================='
                }
            }
        }
        stage('Build Source') {
            steps {
                script {
                    echo '======================================compile begin=============================================='
                    sh '/usr/share/dotnet/dotnet restore --no-cache -s https://api.nuget.org/v3/index.json'
                    sh "/usr/share/dotnet/dotnet build ./${params.PROJECT_FILEPATH} -c Release -o ./app "
                    sh 'ls -alh ./app'
                    echo '======================================compile end================================================'
                }
            }
        }
        stage('Build Images and Push') {
            steps {
                script {
                    echo '======================================Build Images and Push begin=============================================='
                    sh "docker build -f $DOCKERFILE -t $PROJECT_NAME:$BUILD_ID ./app"
                    sh "docker tag $PROJECT_NAME:$BUILD_ID $HARBOR_URL/$HARBOR_GROUP/$PROJECT_NAME:$BUILD_ID"
                    sh "docker push $HARBOR_URL/$HARBOR_GROUP/$PROJECT_NAME:$BUILD_ID"
                    echo '======================================Build Images and Push end================================================'
                }
            }
        }
        stage('Deploy') {
            steps {
                script {
                     sshPublisher(publishers: [sshPublisherDesc(configName: '121.37.104.202', 
                        transfers: [sshTransfer(cleanRemote: false, excludes: '', 
                        execCommand: 'sh /home/jenkins/netcore/deploy.sh $HARBOR_URL $HARBOR_GROUP $PROJECT_NAME $BUILD_ID $HOST_PORT $ASPNETCORE_ENVIRONMENT', 
                        execTimeout: 120000, 
                        flatten: false, 
                        makeEmptyDirs: false,
                        noDefaultExcludes: false,
                        patternSeparator: '[, ]+', 
                        remoteDirectory: '/home/jenkins/netcore', 
                        remoteDirectorySDF: false, removePrefix: '', sourceFiles: './app/deploy.sh')],
                        usePromotionTimestamp: false,
                        useWorkspaceInPromotion: false, verbose: false)]
                     )
                     // sh  "curl -H \"Content-Type:application/json\" -X POST --data '{\"status\": 1}' ${CALLBACK_URL}"
              }
            }
             post {
                success {
                    echo '发布成功'
                }

                failure {
                    echo '发布失败'
                }
            }
        }
    }
}

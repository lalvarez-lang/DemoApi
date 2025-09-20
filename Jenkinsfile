pipeline {
    agent any

    environment {
        DOTNET_ROOT = "${HOME}/.dotnet"
        PATH = "${HOME}/.dotnet:${HOME}/.dotnet/tools:${env.PATH}"
        DOTNET_SYSTEM_GLOBALIZATION_INVARIANT = "1"
        REGISTRY = "demoapiregistry.azurecr.io"
        IMAGE_NAME = "demo-api"
        IMAGE_TAG = GIT_COMMIT.take(7)
        NAMESPACE = "demo-api"
        PRINCIPAL_DIR = "demo-api-helm"
        CHART_DIR  = 'demo-chart'
        DOCS_DIR   = 'docs'
        CHART_NAME = 'demo-chart'
        CHART_PKG_URL = 'https://susanabm.github.io/demo-api-helm/'
        GITOPS_REPO = 'github.com/SusanaBM/demo-api-helm.git'
        VERSION    = '' // se calculará dinámicamente
        NEW_VERSION    = '' // se calculará dinámicamente
    }

    stages {      
        stage('Checkout') {
            steps {
                checkout scm
            }
        }      

        // stage('Install .NET') {
        //     steps {
        //         sh '''
        //             curl -sSL -o install-dotnet.sh https://dot.net/v1/dotnet-install.sh
        //             chmod +x install-dotnet.sh
        //             ./install-dotnet.sh --channel 9.0 --install-dir $DOTNET_ROOT
        //         '''
        //     }
        // }

        // stage('Check .NET') {
        //     steps {
        //         sh 'dotnet --info'
        //     }
        // }

        // stage('Crear Tool Manifest') {
        //     steps {
        //         sh 'dotnet new tool-manifest --force'
        //         echo '✅ Tool manifest creado exitosamente.'
        //     }
        // }

        // stage('Instalar EF Core Tools') {
        //     steps {
        //         sh 'dotnet tool install dotnet-ef'
        //         echo '✅ EF Core Tools instalados exitosamente.'
        //     }
        // }

        // stage('Restaurar Dependencias') {
        //     steps {
        //         sh 'dotnet restore'
        //         sh 'dotnet tool restore'
        //         echo '✅ Dependencias restauradas exitosamente.'
        //     }
        // }

        // stage('Compilar') {
        //     steps {
        //         sh 'dotnet build --configuration Release --no-restore'
        //         echo '✅ Compilación exitosa.'
        //     }
        // }
      
        // stage('Publicar Artefactos') {
        //     steps {
        //         sh 'dotnet publish DemoApi.csproj -c Release -o out'
        //         archiveArtifacts artifacts: 'out/**/*', fingerprint: true
        //         echo '✅ Artefactos publicados exitosamente.'
        //     }
        // }        

        // stage('Login to ACR') {
        //     steps {
        //         withCredentials([usernamePassword(credentialsId: 'acr-creds',
        //                                          usernameVariable: 'AZ_USER',
        //                                          passwordVariable: 'AZ_PASS')]) {
        //             sh """
        //                 echo \$AZ_PASS | docker login $REGISTRY -u \$AZ_USER --password-stdin
        //             """
        //         }
        //     }
        // }

        stage('Build Docker Image') {
            steps {
                sh """
                    docker build -t $REGISTRY/$IMAGE_NAME:$IMAGE_TAG --target final .
                """
            }
        }

        // stage('Build Migration Docker Image') {
        //     steps {
        //         sh """
        //             docker build -f Dockerfile -t $REGISTRY/$IMAGE_NAME:migration --target migration .
        //         """
        //     }
        // }

        stage('Push Docker Image') {
            steps {
                sh """
                    docker push $REGISTRY/$IMAGE_NAME:$IMAGE_TAG
                """
            }
        }

        // stage('Push Migration Docker Image') {
        //     steps {
        //         sh """
        //             docker push $REGISTRY/$IMAGE_NAME:migration
        //         """
        //     }
        // }

        // stage('Clone Chart-GitOps repo') {
        //     steps {
        //         dir("demo-api-helm") {
        //             deleteDir()
        //         }
        //         withCredentials([usernamePassword(credentialsId: 'github-creds-su', usernameVariable: 'GIT_USER', passwordVariable: 'GIT_TOKEN')]) {
                    
        //             sh """
        //                 git clone https://${GITOPS_REPO}
 
        //                 ls -la                        
        //             """
        //         }
               
        //     }
        // }
 
        stage('Package Helm Chart') {
            steps {
                script {
                    // Obtener versión desde Chart.yaml
                    VERSION = sh(
                        script: "grep '^version:' ${PRINCIPAL_DIR}/${CHART_DIR}/Chart.yaml | awk '{print \$2}'",
                        returnStdout: true
                    ).trim()
                    
                    echo "Chart version: ${VERSION}"
                    
                    def (major, minor, patch) = VERSION.tokenize('.').collect { it.toInteger() } // Dividir la versión en partes
                    
                    patch += 1 // Incrementar el patch
                   
                    NEW_VERSION = "${major}.${minor}.${patch}" // Crear la nueva versión

                    echo "Nueva versión: ${NEW_VERSION}"                  

                    withCredentials([usernamePassword(credentialsId: 'github-creds-su', usernameVariable: 'GIT_USER', passwordVariable: 'GIT_TOKEN')]) {
                        sh """
                            sed -i '' 's/^version: .*/version: ${NEW_VERSION}/' ${PRINCIPAL_DIR}/${CHART_DIR}/Chart.yaml

                            cd ${PRINCIPAL_DIR}
                            helm lint ${CHART_DIR}
                            helm dependency update ${CHART_DIR}
                            helm package ${CHART_DIR} -d ${DOCS_DIR}
                            helm repo index ${DOCS_DIR} --url ${CHART_PKG_URL} --merge ${DOCS_DIR}/index.yaml || true

                            if [ ! -d demo-api-helm ]; then
                                git clone https://github.com/SusanaBM/demo-api-helm.git
                            else
                                echo "Repositorio ya existe, actualizando..."
                            fi
                                
                            cd demo-api-helm/demo-chart                                      
            
                            ls -la
            
                            sed -i "s|tagfinal:.*|tagfinal: ${IMAGE_TAG}|" values.yaml                            

                            git config user.email "action@github.com"
                            git config user.name "Github Action"
                            git add values.yaml Chart.yaml
                            git commit -m "Update demo-api image tag to ${IMAGE_TAG}"
                            git remote set-url origin https://${GIT_USER}:${GIT_TOKEN}@github.com/SusanaBM/demo-api-helm.git
                            git push origin main

                        """
                    }
                }
            }
        }



// stage('Incrementar versión') {
//             steps {
//                 script {
//                     // Leer la versión actual del archivo YAML
//                     def version = sh(script: "grep 'version:' config.yaml | awk '{print \$2}'", returnStdout: true).trim()
//                     // Dividir la versión en partes
//                     def (major, minor, patch) = version.tokenize('.').collect { it.toInteger() }
//                     // Incrementar el patch
//                     patch += 1
//                     // Crear la nueva versión
//                     def newVersion = "${major}.${minor}.${patch}"
//                     // Actualizar el archivo YAML
//                     sh """
//                     sed -i 's/version: ${version}/version: ${newVersion}/' config.yaml
//                     sed -i 's/my_image:${version}/my_image:${newVersion}/' config.yaml
//                     """
//                     // Imprimir la nueva versión
//                     echo "Versión actualizada a: ${newVersion}"
//                 }
//             }
//         }
//     }


        // stage('Update GitOps repo') {
        //     steps {
        //         withCredentials([usernamePassword(credentialsId: 'github-creds-su', usernameVariable: 'GIT_USER', passwordVariable: 'GIT_TOKEN')]) {
        //             sh """
        //                 if [ ! -d demo-api-helm ]; then
        //                     git clone https://github.com/SusanaBM/demo-api-helm.git
        //                 else
        //                     echo "Repositorio ya existe, actualizando..."
        //                 fi
                        
        //                 cd demo-api-helm/demo-chart                                      
        
        //                 ls -la
        
        //                 sed -i "s|tagfinal:.*|tagfinal: ${IMAGE_TAG}|" values.yaml
                        

        //                 git config user.email "action@github.com"
        //                 git config user.name "Github Action"
        //                 git add values.yaml Chart.yaml
        //                 git commit -m "Update demo-api image tag to ${IMAGE_TAG}"
        //                 git remote set-url origin https://${GIT_USER}:${GIT_TOKEN}@github.com/SusanaBM/demo-api-helm.git
        //                 git push origin main
        //             """
        //         }
               
        //     }
        // }
    }

    post {
        success {
            echo '✅ Pipeline completado con éxito.'
        }
        failure {
            echo '❌ Pipeline falló.'
        }
    }
}

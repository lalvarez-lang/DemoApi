pipeline {
    agent any

    environment {
        DOTNET_ROOT = "${HOME}/.dotnet"
        PATH = "${HOME}/.dotnet:${HOME}/.dotnet/tools:${env.PATH}"
        DOTNET_SYSTEM_GLOBALIZATION_INVARIANT = "1"
        REGISTRY = "aksdemo2025registry.azurecr.io"
        IMAGE_NAME = "demo-api"
        IMAGE_TAG = "latest"
        NAMESPACE = "demo-api"
    }

    stages {      
        stage('Checkout') {
            steps {
                checkout scm
            }
        }      

        stage('Install .NET') {
            steps {
                sh '''
                    curl -sSL -o install-dotnet.sh https://dot.net/v1/dotnet-install.sh
                    chmod +x install-dotnet.sh
                    ./install-dotnet.sh --channel 9.0 --install-dir $DOTNET_ROOT
                '''
            }
        }

        stage('Check .NET') {
            steps {
                sh 'dotnet --info'
            }
        }

        stage('Crear Tool Manifest') {
            steps {
                sh 'dotnet new tool-manifest --force'
                echo '✅ Tool manifest creado exitosamente.'
            }
        }

        stage('Instalar EF Core Tools') {
            steps {
                sh 'dotnet tool install dotnet-ef'
                echo '✅ EF Core Tools instalados exitosamente.'
            }
        }

        stage('Restaurar Dependencias') {
            steps {
                sh 'dotnet restore'
                sh 'dotnet tool restore'
                echo '✅ Dependencias restauradas exitosamente.'
            }
        }

        stage('Compilar') {
            steps {
                sh 'dotnet build --configuration Release --no-restore'
                echo '✅ Compilación exitosa.'
            }
        }

        stage('Ejecutar Pruebas') {
            steps {
                sh 'dotnet test --configuration Release --no-build --verbosity normal'
                echo '✅ Pruebas ejecutadas exitosamente.'
            }
        }

        stage('Publicar Artefactos') {
            steps {
                sh 'dotnet publish DemoApi.csproj -c Release -o out'
                archiveArtifacts artifacts: 'out/**/*', fingerprint: true
                echo '✅ Artefactos publicados exitosamente.'
            }
        }        

        stage('Login to ACR') {
            steps {
                withCredentials([usernamePassword(credentialsId: 'acr-creds-demo',
                                                 usernameVariable: 'AZ_USER',
                                                 passwordVariable: 'AZ_PASS')]) {
                    sh """
                        echo \$AZ_PASS | docker login $REGISTRY -u \$AZ_USER --password-stdin
                    """
                }
            }
        }

        stage('Build Docker Image') {
            steps {
                sh """
                    docker build -t $REGISTRY/$IMAGE_NAME:$IMAGE_TAG --target final .
                """
            }
        }

        stage('Build Migration Docker Image') {
            steps {
                sh """
                    docker build -f Dockerfile -t $REGISTRY/$IMAGE_NAME:migration --target migration .
                """
            }
        }

        stage('Push Docker Image') {
            steps {
                sh """
                    docker push $REGISTRY/$IMAGE_NAME:$IMAGE_TAG
                """
            }
        }

        stage('Push Migration Docker Image') {
            steps {
                sh """
                    docker push $REGISTRY/$IMAGE_NAME:migration
                """
            }
        }

        stage('Run EF Migrations in Cluster') {
            steps {
                withCredentials([file(credentialsId: 'kubeconfig-api-demo', variable: 'KUBECONFIG')]) {
                    sh '''
                    # Ejecutar migraciones desde un pod temporal en el cluster
                    kubectl run ef-migrate --rm -i -n $NAMESPACE \
                      --image=$REGISTRY/$IMAGE_NAME:migration --restart=Never \
                      --env="ConnectionStrings__DefaultConnection=Host=51.57.57.170;Port=5432;Database=pedidosdb;Username=demoapi;Password=Qwerty123;Ssl Mode=Require;Trust Server Certificate=true;" --command -- \
                      /bin/bash -c "dotnet tool restore && dotnet ef database update --project DemoApi.csproj --startup-project DemoApi.csproj"
                    '''
                }
            }
        }
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

pipeline {
    agent any

    environment {
        DOTNET_ROOT = '/usr/share/dotnet'
        PATH = "${DOTNET_ROOT}:${DOTNET_ROOT}/tools:${PATH}"
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Setup .NET') {
            steps {
                script {
                    // Проверяем установлен ли .NET
                    sh 'dotnet --version || echo ".NET not installed, installing..."'
                    
                    // Установка .NET если нужно (для Linux агентов)
                    sh '''
                    if ! command -v dotnet &> /dev/null; then
                        echo "Installing .NET SDK..."
                        wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
                        chmod +x dotnet-install.sh
                        ./dotnet-install.sh --channel LTS
                        export DOTNET_ROOT=$HOME/.dotnet
                        export PATH=$DOTNET_ROOT:$DOTNET_ROOT/tools:$PATH
                    fi
                    '''
                }
            }
        }

        stage('Restore NuGet Packages') {
            steps {
                sh 'dotnet restore'
            }
        }

        stage('Build Solution') {
            steps {
                sh 'dotnet build --configuration Release --no-restore'
            }
        }

        stage('Run Tests') {
            steps {
                sh 'dotnet test --configuration Release --no-build --verbosity normal'
            }
        }

        stage('Publish Application') {
            steps {
                sh 'dotnet publish --configuration Release --output ./publish --no-build'
            }
        }

        stage('Archive Artifacts') {
            steps {
                archiveArtifacts 'publish/**/*'
                // Или для конкретных файлов:
                archiveArtifacts '**/bin/Release/**/*.dll'
                archiveArtifacts '**/bin/Release/**/*.exe'
            }
        }
    }

    post {
        always {
            echo "Build completed - cleaning up"
            // Очистка временных файлов
            sh 'dotnet clean || true'
        }
        success {
            echo 'C# Build completed successfully! ✅'
        }
        failure {
            echo 'C# Build failed! ❌'
        }
    }
}
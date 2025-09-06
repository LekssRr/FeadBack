pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Check .NET') {
            steps {
                script {
                    // Проверяем установлен ли .NET
                    def dotnetInstalled = sh(script: 'command -v dotnet', returnStatus: true) == 0
                    
                    if (!dotnetInstalled) {
                        error ".NET SDK не установлен на агенте. Установите .NET 8.0 SDK на сервер Jenkins"
                    }
                    
                    sh 'dotnet --version'
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
            }
        }
    }

    post {
        success {
            echo 'C# Build completed successfully! ✅'
        }
        failure {
            echo 'C# Build failed! ❌'
        }
    }
}
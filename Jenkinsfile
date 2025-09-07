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
                    sh 'dotnet --version'
                }
            }
        }

        stage('Restore NuGet Packages') {
            steps {
                // Выберите один из вариантов:
                
                // Вариант 1: Если есть solution файл
                sh 'dotnet restore FeedBack.sln'
                
                // Вариант 2: Если есть конкретный проект
                sh 'dotnet restore src/FeedBack.API/FeedBack.API.csproj'
                
                // Вариант 3: Перейти в директорию проекта
                dir('src/FeedBack.API') {
                    sh 'dotnet restore'
                }
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

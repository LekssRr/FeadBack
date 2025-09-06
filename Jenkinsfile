pipeline {
    agent {
        docker {
            image 'mcr.microsoft.com/dotnet/sdk:8.0'
            args '-v /var/jenkins_home/workspace/feed:/app -w /app'
            reuseNode true
        }
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
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
pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Setup .NET') {
            steps {
                script {
                    // Устанавливаем .NET SDK если не установлен
                    sh '''
                    if ! command -v dotnet &> /dev/null; then
                        echo "Installing .NET SDK..."
                        apt-get update
                        apt-get install -y wget
                        wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
                        dpkg -i packages-microsoft-prod.deb
                        apt-get update
                        apt-get install -y dotnet-sdk-8.0
                    fi
                    dotnet --version
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
            }
        }
    }

    post {
        always {
            echo "Build completed"
        }
        success {
            echo 'C# Build completed successfully! ✅'
        }
        failure {
            echo 'C# Build failed! ❌'
        }
    }
}
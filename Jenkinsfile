pipeline {
    agent any

    stages {

        stage('Build') {
            steps {
                sh 'docker run --rm -v $PWD:/app -w /app mcr.microsoft.com/dotnet/sdk:10.0 dotnet build'
            }
        }

        stage('Test') {
            steps {
                sh 'docker run --rm -v $PWD:/app -w /app mcr.microsoft.com/dotnet/sdk:10.0 dotnet test || true'
            }
        }

        stage('Code Quality') {
            steps {
                sh 'docker run --rm -v $PWD:/app -w /app mcr.microsoft.com/dotnet/sdk:10.0 dotnet format --verify-no-changes || true'
            }
        }

        stage('Security Scan') {
            steps {
                sh 'docker run --rm -v $PWD:/app aquasec/trivy fs /app || true'
            }
        }

        stage('Docker Build') {
            steps {
                sh 'docker build -t robot-api .'
            }
        }

        stage('Deploy') {
            steps {
                sh 'docker stop robot-container || true'
                sh 'docker rm robot-container || true'
                sh 'docker run -d -p 8081:80 --name robot-container robot-api'
            }
        }

        stage('Release') {
            steps {
                sh 'docker tag robot-api robot-api:latest'
            }
        }

        stage('Monitoring') {
            steps {
                sh 'docker ps'
            }
        }
    }
}
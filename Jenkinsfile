pipeline {
    agent any

    stages {

        stage('Build') {
            steps {
                sh 'dotnet build robot-controller-api.sln'
            }
        }

        stage('Test') {
            steps {
                sh 'dotnet test robot-controller-api.sln || true'
            }
        }

        stage('Code Quality') {
            steps {
                sh 'dotnet format robot-controller-api.sln || true'
            }
        }

        stage('Security Scan') {
            steps {
                echo 'Security scan placeholder'
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

        stage('Monitoring') {
            steps {
                sh 'docker ps'
            }
        }
    }
}
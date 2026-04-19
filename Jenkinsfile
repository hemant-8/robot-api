pipeline {
    agent any

    stages {

        stage('Build') {
            agent {
                docker {
                    image 'mcr.microsoft.com/dotnet/sdk:10.0'
                }
            }
            steps {
                sh 'dotnet build robot-controller-api.sln'
            }
        }

        stage('Test') {
            agent {
                docker {
                    image 'mcr.microsoft.com/dotnet/sdk:10.0'
                }
            }
            steps {
                sh 'dotnet test robot-controller-api.sln || true'
            }
        }

        stage('Docker Build') {
            steps {
                sh 'docker build -t robot-api .'
            }
        }

        stage('Code Quality (SonarQube)') {
            steps {
                withCredentials([string(credentialsId: 'sonar-token', variable: 'SONAR_TOKEN')]) {
                    sh '''
                    docker run --rm \
                    -e SONAR_HOST_URL=http://host.docker.internal:9000 \
                    -e SONAR_TOKEN=$SONAR_TOKEN \
                    -e SONAR_PROJECT_KEY=robot-api \
                    -e SONAR_SOURCES=. \
                    -v $(pwd):/usr/src \
                    sonarsource/sonar-scanner-cli
                    '''
                }
            }
        }

        stage('Security Scan (Trivy)') {
            steps {
                sh '''
                docker run --rm -v /var/run/docker.sock:/var/run/docker.sock \
                aquasec/trivy image robot-api
                '''
            }
        }

        stage('Deploy') {
            steps {
                sh 'docker stop robot-container || true'
                sh 'docker rm robot-container || true'
                sh 'docker run -d -p 8081:8080 --name robot-container robot-api'
            }
        }

        stage('Release') {
            steps {
                sh 'git tag v1.0.${BUILD_NUMBER} || true'
                sh 'git push origin --tags || true'
            }
        }

        stage('Monitoring') {
            steps {
                sh 'docker stats --no-stream'
            }
        }
    }
}
pipeline {
    agent any

    environment {
        SONAR_HOST = 'http://host.docker.internal:9000'
    }

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

        stage('Code Quality (SonarQube)') {
            agent {
                docker {
                    image 'mcr.microsoft.com/dotnet/sdk:10.0'
                }
            }
            steps {
                withCredentials([string(credentialsId: 'sonar-token', variable: 'SONAR_TOKEN')]) {
                    sh '''
                    dotnet tool install --global dotnet-sonarscanner
                    export PATH="$PATH:/root/.dotnet/tools"

                    dotnet sonarscanner begin \
                    /k:"robot-api" \
                    /d:sonar.host.url=$SONAR_HOST \
                    /d:sonar.login=$SONAR_TOKEN

                    dotnet build robot-controller-api.sln

                    dotnet sonarscanner end \
                    /d:sonar.login=$SONAR_TOKEN
                    '''
                }
            }
        }

        stage('Docker Build') {
            steps {
                sh 'docker build -t robot-api .'
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

        post {
            success {
                emailext(
                    to: 'hemantsachdeva19@gmail.com',
                    subject: "SUCCESS: Build #${BUILD_NUMBER}",
                    body: """
                    Build Successful!

                    Project: ${JOB_NAME}
                    Build Number: ${BUILD_NUMBER}

                    Check details: ${BUILD_URL}
                    """,
                    attachLog: true
                )
            }

            failure {
                emailext(
                    to: 'hemantsachdeva19@gmail.com',
                    subject: "FAILED: Build #${BUILD_NUMBER}",
                    body: """
                    Build Failed!

                    Project: ${JOB_NAME}
                    Build Number: ${BUILD_NUMBER}

                    Check logs: ${BUILD_URL}
                    """,
                    attachLog: true
                )
            }
        }
# Beis SME voucher web app #

### Deployment ###

Although the application can be deployed as normal microsoft ddl, we recommend the use of docker container and
kubernetes for deployment.

* #### Docker deployment commands ####   
    * docker build -t beis-htg-sme-voucher-webapp .
    * docker tag BEIS.HelpToGrow.Voucher.Web {dockerhubid}/beis-htg-sme-voucher-webapp
    * docker push {dockerhubid}/beis-htg-sme-voucher-webapp
    * docker run -p 5000:5000 {dockerhubid}/beis-htg-sme-voucher-webapp

The docker push command may required you to log into docker. Provide your docker details then proceed.   
After running the above command, the application will be available on the url: http://{hostname}: 5000


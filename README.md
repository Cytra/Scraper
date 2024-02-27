# Start Selenium Locally

docker run -d -p 4444:4444 -p 7900:7900 --shm-size="2g" selenium/standalone-chrome:latest

you can see what docker is doing on http://localhost:7900 password => secret

# Deploy Selenium to K8s cluster

cd C:\Users\martynas.samuilovas\source\Personal\Scraper\selenium-chart
helm install selenium-release docker-selenium/selenium-grid -f values.yaml --namespace selenium
helm install selenium-grid docker-selenium/selenium-grid
helm uninstall selenium-grid

# Update the deployment
helm upgrade selenium-release docker-selenium/selenium-grid -f values.yaml --namespace selenium

helm delete selenium-release --namespace selenium

#
FROM mcr.microsoft.com/dotnet/sdk:6.0-bullseye-slim

RUN dotnet tool install --global dotnet-ef --version 6.0
ENV PATH $PATH:/root/.dotnet/tools

WORKDIR /app

# Copy wait-for-it.sh to wait for sql before running integration tests
ADD https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh /wait-for-it.sh
# Make it executable, in Linux
RUN chmod +x /wait-for-it.sh

COPY ["scripts/tests/run-tests.sh", "/"]
RUN chmod +x /run-tests.sh
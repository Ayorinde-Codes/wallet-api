FROM microsoft/dotnet:2.2-sdk as build

ARG BUILDCONFIG=RELEASE
ARG VERSION=1.0.0

COPY Numero.csproj /build/

RUN dotnet restore ./build/Numero.csproj

COPY . ./build/
WORKDIR /build/
RUN dotnet publish ./Numero.csproj -c $BUILDCONFIG -o out /p:Version=$VERSION

FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app

COPY --from=build /build/out .

ENTRYPOINT ["dotnet", "Numero.dll"] 

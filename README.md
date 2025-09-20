# Trabajo Final - Diplomado de Arquitectura de Software

## Descripción
Este repositorio contiene el código para un sistema de gestión de pedidos, implementado con Helm y gestionado por ArgoCD.

## Instalación del Chart con Helm

Para instalar el chart en el cluster, y actualizar los archivos Helm:

```bash
helm repo add demo-repo https://susanabm.github.io/demo-api-helm/

helm repo update

helm install demo-api demo-repo/demo-chart -n demo-api --create-namespace

```

## Sincronizacion ArgoCD 

Argo está instalado en el clúster dentro del namespace argocd para la API (Demo-api) apuntando a un
repositorio Git donde se encuentra el chart Helm de la app (demo-api-helm).
 
La Application usa como destino el clúster y despliega en el namespace demo-api. La fuente está declarada como Helm,
con release name demo-api y los valores tomados de values.yaml.
 
Con esto, Argo vigila el repo y cuando detecta cambios en el chart o en los valores renderiza el Helm y aplica los manifiestos
resultantes al namespace demo-api del clúster de destino, dejando el estado “Synced/Healthy” cuando los recursos (Deployment, Service e Ingress) están creados.

## Endpoints

Se instalo en el back end la herramienta swagger para la ejecucion de pruebas el cual puede ser visualizado en la siguiente url 

```
    http://4.227.87.116/swagger/index.html
```

Desde esta url se podran testear los siguientes endpoints

Controller - Pedidos

1. Endpoint: /api/Pedidos 
    * Description: Permite la entrada de un registro a la base de datos
    * Method: POST
    * Payload:

    ```Json
        {
            "fecha": "2025-09-20T02:22:46.963Z"
        }
    ```

    * Response

    ```Json
        {
             "id": "7b5e7534-858a-4407-b8ca-569be3482da6",
            "fecha": "2025-09-20T02:22:46.963Z"
        }
    ```

2. Endpoint: /api/Pedidos 
    * Description: Permite listar los diferentes recods almacenados en la tabla pedidos
    * Method: GET
    
    * Response
    
    ```Json
        [
            {
                "id": "7b5e7534-858a-4407-b8ca-569be3482da6",
                "fecha": "2025-09-20T02:22:46.963Z"
            },
            {
                "id": "d8179858-f923-45ff-8913-663fce20d99b",
                "fecha": "2025-09-20T02:22:46.963Z"
            },
            {
                "id": "8c782c17-0be1-437d-8937-fea96e4952a1",
                "fecha": "2025-09-20T02:22:46.963Z"
            }
        ]
    ```
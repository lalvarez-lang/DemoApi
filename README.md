# Trabajo Final - Diplomado de Arquitectura de Software

## Descripción
Este repositorio contiene el código para un sistema de gestión de pedidos, implementado con Helm y gestionado por ArgoCD.

## Instalación del Chart con Helm
Para instalar el chart manualmente, y actualizar los archivos Helm:
```bash
helm repo update;  # Al realizar el cambio se actualiza con este comando

helm dependency build ./DemoApiChart/DemoApi; #este permite ejecutar la depuracion

helm install DemoApi ./DemoApiChart/DemoApi #aplicar los cambios

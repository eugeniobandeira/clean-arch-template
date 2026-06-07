# Kubernetes

This folder contains Kubernetes manifests for deploying the application.

## Structure

```
k8s/
├── base/
│   ├── deployment.yaml
│   ├── service.yaml
│   └── kustomization.yaml
└── overlays/
    ├── dev/
    │   └── kustomization.yaml
    └── prod/
        └── kustomization.yaml
```

## Usage

Apply to a cluster:

```bash
kubectl apply -k k8s/overlays/dev
kubectl apply -k k8s/overlays/prod
```

#!/bin/bash
ResourceGroup="Gratify"

az group create \
    --verbose \
    --location northeurope \
    --name $ResourceGroup

az group deployment create \
    --verbose \
    --resource-group $ResourceGroup \
    --template-file arm-template.json \
    --parameters resourceGroup=$ResourceGroup sqlAdminPassword='<REPLACE_WITH_GITHUB_ENV>'
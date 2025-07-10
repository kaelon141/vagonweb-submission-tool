<script setup lang="ts">
import { computed, ref } from 'vue'
import { useQuery } from '@tanstack/vue-query'
import Skeleton from "primevue/skeleton";
import TreeSelect from "primevue/treeselect";
import type { TreeExpandedKeys, TreeSelectionKeys, TreeNode } from "primevue/tree"
import FloatLabel from "primevue/floatlabel";

import { removeDiacritics } from "../utils/removeDiacritics.ts";

const selectedKeys = ref<TreeSelectionKeys>({});
const expandedKeys = ref<TreeExpandedKeys>({});
const searchLabel = ref('');

const { isPending, isError, data, error } = useQuery({
  queryKey: ['trainCategories'],
  queryFn: async () => {
    const response = await fetch('https://localhost:7223/api/v1/trains/categories');
    return response.json();
  }
})

const options = computed(() => {
  if (isPending.value) return;

  const options = data.value.map(country => ({
    key: country.countryName,
    label: country.countryName,
    data: country.countryName,
    selectable: false,
    country: country.countryName,
    children: country.operators.map(operator => ({
      key: operator.operatorCode,
      data: operator.operatorCode,
      label: operator.name,
      selectable: true,
      country: country.countryName,
      operatorCode: operator.operatorCode,
      operatorName: operator.name,
      children: operator.categories.map(category => ({
        key:  `${operator.operatorCode} ${category.categoryCode}`,
        data: category.categoryCode,
        label: category.description,
        selectable: true,
        country: country.countryName,
        operatorCode: operator.operatorCode,
        operatorName: operator.name,
        categoryCode: category.categoryCode,
        categoryName: category.description,
      }))
    }))
  }));

  options.sort((a, b) => a.label.localeCompare(b.label));
  return options;
})

const selectedOption = defineModel('');

const nodeFilterString = (node) =>
  `${node.country} ${node.operatorCode} ${node.categoryCode} ${removeDiacritics(node.country)} ${removeDiacritics(node.operatorCode)} ${removeDiacritics(node.categoryCode)} ${node.operatorCode} ${node.categoryName} ${node.operatorName} ${node.categoryCode} ${node.operatorName} ${node.categoryName} !${removeDiacritics(node.operatorCode)} ${removeDiacritics(node.categoryName)} ${removeDiacritics(node.operatorName)} ${removeDiacritics(node.categoryCode)} ${removeDiacritics(node.operatorName)} ${removeDiacritics(node.categoryName)} `;

const pt = {
  pcTree: {
    pcFilterInput: ({state}) => {
      if (state.filterValue != null) {
        filterValueChanged(state.filterValue);
      }
    }
  }
}

const filterValueChanged = (value: string | null) => {
  if (value && value?.trim().length >= 1) {
    // Expand all nodes when a new value is entered in the text field
    if (searchLabel.value != value) {
      searchLabel.value = value;
      expandAll();
    }
    // Collapse all nodes when value is erased
  } else if (!allSiteCollapsed() && value != null && value?.trim().length === 0 && searchLabel.value.length != 0) {
    collapseAll();
    searchLabel.value = '';
  }
}

const expandNode = (node: TreeNode) => {
  if (node.children && node.children.length && node.key) {
    expandedKeys.value[node.key] = true;

    for (const child of node.children) {
      expandNode(child);
    }
  }
}

const expandAll = () => {
  for (const node of options.value) {
    expandNode(node);
  }
}

const collapseNode = (node: TreeNode, currentExpandedSiteKey: TreeSelectionKeys) => {
  if (node.children && node.children.length && node.key) {
    currentExpandedSiteKey[node.key] = false;

    for (const child of node.children) {
      collapseNode(child, currentExpandedSiteKey);
    }
  }
}

const collapseAll = () => {
  const currentExpandedSiteKey = expandedKeys.value;
  for (const node of options.value) {
    collapseNode(node, currentExpandedSiteKey);
  }

  expandedKeys.value = currentExpandedSiteKey;
}

const allSiteCollapsed = () => {
  for (const prop in expandedKeys.value) {
    if (expandedKeys.value[prop]) {
      return false;
    }
  }
  return true;
}

</script>

<template>
  <Skeleton v-if="!options" />
  <FloatLabel v-else variant="in">
    <TreeSelect id="category" v-model="selectedOption" :options="options" class="w-full" filter filter-mode="strict" :filter-by="nodeFilterString" :multiple="false" :pt="pt" v-model:expanded-keys="expandedKeys" @update:model-value="collapseAll">
      <template v-slot:option="{ node }">
        <span v-if="node.categoryCode"><strong>{{node.categoryCode}}</strong> {{node.label}}</span>
        <span v-else-if="node.key !== node.label"><strong>{{node.key}}</strong> {{ node.label }}</span>
        <span v-else>{{ node.label }}</span>
      </template>
      <template v-slot:value="{ value: [node] }">
        <span v-if="!!node">
          <strong>{{node.operatorCode}}</strong>
          <strong v-if="node.categoryCode">&nbsp;{{node.categoryCode}}</strong>&nbsp;{{node.label}}
        </span>
      </template>
    </TreeSelect>
    <label for="category">Train operator or category</label>
  </FloatLabel>
</template>

<style scoped>

</style>
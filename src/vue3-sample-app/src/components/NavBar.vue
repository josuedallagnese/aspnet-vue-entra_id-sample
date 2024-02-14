<template>
  <el-menu class="el-menu-demo" mode="horizontal" background-color="#545c64" text-color="#fff">
        <el-menu-item index="home">
            <i class="el-icon-house"></i>
            <router-link to="/">Microsoft Identity Platform</router-link>
        </el-menu-item>
        <el-menu-item index="profile">
            <i class="el-icon-house"></i>
            <router-link to="/profile">Profile</router-link>
        </el-menu-item>
        <el-menu-item index="buttonGroup" id="authButton">
            <SignOutButton v-if="isAuthenticated"/>
            <SignInButton v-else />
        </el-menu-item>
  </el-menu>
</template>

<script setup lang="ts">
import { useIsAuthenticated } from '../composition-api/useIsAuthenticated';
import SignInButton from "./SignInButton.vue";
import SignOutButton from "./SignOutButton.vue";

const isAuthenticated = useIsAuthenticated();
</script>

<style>
a {
    text-decoration: none;
    color: #fff;
}
#authButton {
    margin-left: auto;
}
</style>
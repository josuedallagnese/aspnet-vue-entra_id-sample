<template>
	<div v-if="state.resolved">
		<p>Name: {{ state.data.displayName }}</p>
		<p>Title: {{ state.data.jobTitle }}</p>
		<p>Mail: {{ state.data.mail }}</p>
		<p>Phone: {{ state.data.businessPhones ? state.data.businessPhones[0] : "" }}</p>
		<p>Location: {{ state.data.officeLocation }}</p>
		<br/>
		<p><strong>{{ state.welcomeMessage.welcomeMessage }}</strong></p>
	</div>
</template>

<script setup lang="ts">
import { useMsal } from "../composition-api/useMsal";
import { InteractionRequiredAuthError, InteractionStatus } from "@azure/msal-browser";
import { reactive, onMounted, watch } from 'vue'
import { loginRequest, loginRequestProfile, securityGroups } from "../authConfig";
import { callMsGraph } from "../utils/MsGraphApiCall";
import { getWellcomeMessage } from '../utils/BackendApiCall';
import UserInfo from "../utils/UserInfo";

const { instance, inProgress } = useMsal();

const state = reactive({
	resolved: false,
	data: {} as UserInfo,
	welcomeMessage: {} as any
});

async function loadProfile() {
	const response = await instance.acquireTokenSilent({
		...loginRequestProfile
	}).catch(async (e) => {
		if (e instanceof InteractionRequiredAuthError) {
			await instance.acquireTokenRedirect(loginRequestProfile);
		}
		throw e;
	});

	if (inProgress.value === InteractionStatus.None) {
		state.data = await callMsGraph(response.accessToken);
		state.resolved = true;
		stopWatcher();
	}
}

async function loadWelcomeMessage() {
	const response = await instance.acquireTokenSilent({
		...loginRequest
	}).catch(async (e) => {
		if (e instanceof InteractionRequiredAuthError) {
			await instance.acquireTokenRedirect(loginRequest);
		}
		throw e;
	});

	if (inProgress.value === InteractionStatus.None) {

		const account = instance.getActiveAccount();

		if (account?.idTokenClaims && account?.idTokenClaims?.groups) {

			const groups: any = account.idTokenClaims.groups;
			let endpoint: string = '';

			if (groups.includes(securityGroups.development.id)) {
				endpoint = securityGroups.development.endpoint;
			} else if (groups.includes(securityGroups.productOwner.id)) {
				endpoint = securityGroups.productOwner.endpoint;
			}

			state.welcomeMessage = await getWellcomeMessage(endpoint, response.accessToken);
		}

		state.resolved = true;
		stopWatcher();
	}
}

onMounted(() => {
	loadProfile();
	loadWelcomeMessage();
});

const stopWatcher = watch(inProgress, () => {
	if (!state.resolved) {
		loadProfile();
		loadWelcomeMessage();
	}
});
</script>

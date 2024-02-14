import { LogLevel, PublicClientApplication } from '@azure/msal-browser';

export const msalConfig = {
  auth: {
    clientId: '',
    authority: 'https://login.microsoftonline.com/',
    redirectUri: '/',
    postLogoutRedirectUri: '/',
  },
  cache: {
    cacheLocation: 'localStorage'
  },
  system: {
    loggerOptions: {
      loggerCallback: (level: LogLevel, message: string, containsPii: boolean) => {
        if (containsPii) {
          return;
        }
        switch (level) {
          case LogLevel.Error:
            console.error(message);
            return;
          case LogLevel.Info:
            console.info(message);
            return;
          case LogLevel.Verbose:
            console.debug(message);
            return;
          case LogLevel.Warning:
            console.warn(message);
            return;
          default:
            return;
        }
      },
      logLevel: LogLevel.Error
    }
  }
};

export const msalInstance = new PublicClientApplication(msalConfig);

export const loginRequest = {
  scopes: []
};

export const loginRequestProfile = {
  scopes: []
};

export const graphConfig = {
  graphMeEndpoint: 'https://graph.microsoft.com/v1.0/me',
};

export const securityGroups = {
  development: { id: '', endpoint: 'http://localhost:9000/development' },
  productOwner: { id: '', endpoint: 'http://localhost:9000/product-owner' },
};
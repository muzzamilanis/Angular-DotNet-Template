export interface LoginDto {
  userNameOrEmailAddress: string;
  password: string;
  rememberMe: boolean;
}

export interface AuthenticateResultModel {
  accessToken: string;
  encryptedAccessToken: string;
  expireInSeconds: number;
  userId: number;
}
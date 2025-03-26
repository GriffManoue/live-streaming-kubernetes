export interface AuthResult {
    success: boolean;
    token?: string;
    error?: string;
    userId?: string;
    expiresAt?: Date;
  }
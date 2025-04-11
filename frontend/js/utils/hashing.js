export async function generateClientHash(password) {
    const clientSalt = 'fixed_client_salt_!@#';
    
    return sha256(password + clientSalt);
}    
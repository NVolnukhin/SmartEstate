import { config } from "../config.js"; 

export async function generateClientHash(password) {    
    return sha256(password + config.hash.salt);
}    
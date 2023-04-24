/** @type {import('next').NextConfig} */
const nextConfig = {
  reactStrictMode: true,
  env: {
  	requesturl: process.env.requesturl
  }
}

module.exports = nextConfig

require("@nomicfoundation/hardhat-toolbox");

/** @type import('hardhat/config').HardhatUserConfig */
module.exports = {
  defaultNetwork: "hardhat",
  networks: {
    hardhat: {
      //gas: auto, // not used for ethers
      //gasPrice: 225000000000, // not used for ethers
      initialBaseFeePerGas: 1500000000, // 1.5 gwei
      //initialBaseFeePerGas: 10,
      blockGasLimit: 30000000,
      chainId: 43112,
    },
    testing: {
      url: "http://127.0.0.1:8545",
      allowUnlimitedContractSize: true,
      initialBaseFeePerGas: 10, //1500000000, // 1.5 gwei
    }
  },
  solidity: "0.8.18",
  paths: {
    sources: "./contracts",
    tests: "./hardhat-test",
    cache: "./hardhat-cache",
    artifacts: "./hardhat-artifacts"
  },
};

<template>
  <div class="lk row">
    <sidebar class="col-xl-3" :user="user"/>
    <Body class="col-xl-9" />
  </div>
</template>

<script>
import Sidebar from './components/Sidebar';
import Body from './components/Body';

export default {
  name: 'app',
  data() {
    return {      
      user: {
        id: 0,
        steamid: "",
        profileName: "",
        profileImg: "",
        balance: 0,
        inventory: null
      }
    }
  },
  methods: {
    isDev(){
      return process.env.NODE_ENV == "development";
    },
    async GetData(){
      const url = "/api/Spa/GetUserInfo";      
      let resp = await fetch(url);           
      if(resp.ok){
       this.user = await resp.json();
      }else window.location.href = "/";
    }
  },
  components:{
    Sidebar,
    Body
  },
  created(){
    if(this.isDev()){
      this.user = {
        Id: 1,
        Steamid: "76561198208390417",
        ProfileName: "SirEleot",
        ProfileImg: "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/80/80b9eaafc8aa816019c1678e20430374a98c300a_full.jpg",
        Balance: 0,
        Inventory: null
      }
    }else{
      this.GetData() 
    }
  }
}
</script>

<style lang="scss">
@import './colors';
  .lk{
    width: 100%; 
    min-height: 600px;
    background-color: rgba($clr_2, .5);
    border-radius: 5px;
    box-shadow: 2vh 2vh 4vh 1px $clr_2;
    border: 1px solid $clr_3;
    display: flex;
  }
</style>


<template>
  <div class="lk row">
    <Dialog v-show="dialog" :message="message" />
    <Sidebar class="col-xl-3" :user="user"/>
    <Body class="col-xl-9" :inventory="user.inventory" @onAction="actionBegine" :price="price"/>
  </div>
</template>

<script>
import Sidebar from './components/Sidebar';
import Body from './components/Body';
import Dialog from './components/Dialog';

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
        inventory: []
      },
      action: true,
      dialog: false,
      message:{
        tittle: '',
        text: '',
        btn1: {
          text: '',
          action: ()=>{}
        },
        btn2: {
          text: '',
          action: ()=>{}
        }
      },
      price:{
        sex: 200,
        position: 100,
        slot: 150
      }
    }
  },
  watch:{
    dialog(newVal){
      const body = document.getElementById('body');
      //window.console.log(oldVal, newVal);
      if(newVal) body.classList.add('disabled');
      else body.classList.remove('disabled');
    }
  },
  methods: {
    isDev(){
      return process.env.NODE_ENV == "development";
    },
    async actionBegine(type, id, val){
      if(this.action || this.dialog) return;
      let action, msg;
      switch (type) {
        case 'pos':
          if(this.price.position > this.user.balance) return this.infoMessage("Недостаточно средств");
          msg = `Вы хотите телепортировать динозавра за ${this.price.position} DC?`
          action = async ()=>{     
            let url = `/api/Spa/SetPosition?id=${id}&pos=${val}`;   
            this.request(url);
          }
          break;

        case 'activate':
          msg = `Вы хотите активировать этого динозавра?`
          action = async ()=>{
            let url = `/api/Spa/ActivateDino?id=${id}`;      
            this.request(url);
          } 
          break;

        case 'slot':
          if(this.price.slot > this.user.balance) return this.infoMessage("Недостаточно средств");
          msg = `Вы хотите добавить дополнительный слот для динозавра за ${this.price.slot} DC?`
          action = async ()=>{
            let url = `/api/Spa/AddSlot`;      
            this.request(url);
          }  
          break;

        case 'sex':
          if(this.price.sex > this.user.balance) return this.infoMessage("Недостаточно средств");
          msg = `Желаете сменить пол этому диназавру за ${this.price.sex} DC?`
          action = async ()=>{
            let url = `/api/Spa/ChangeSex?id=${id}`;      
            this.request(url);
          } 
          break;

        case 'delete':
          msg = `Желаете удалить навсегда этого диназавра?`
          action = async ()=>{
            let url = `/api/Spa/DeleteDino?id=${id}`;      
            this.request(url);
          }  
          break;
        default: return this.infoMessage("Неизвестный запрос");
      }    
      this.confirmMessage(msg, action)
    },
    async request(url){
      this.action = true;
      this.loadingMessage();
      let resp = await fetch(url);
      if(resp.ok) {
        let result = await resp.json();
        this.infoMessage(result.message, result.error);  
        if(!result.error) this.getData();
      }
      else {
        this.infoMessage('Неверный ответ от сервера');
      }      
      this.action = false;
    },
    async getData(){
      if(this.isDev()){
        this.user = {
          id: 1,
          steamid: "76561198208390417",
          profileName: "SirEleot",
          profileImg: "https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/80/80b9eaafc8aa816019c1678e20430374a98c300a_full.jpg",
          balance: 5000,
          slots: 3,
          inventory: [
            {"id":2,"name":"Раптор","image":"TestImg.png","isAlive":true,"isActivated":false,"vip":true,"craetionAs":"2020-01-04T13:13:32.216569"},
            {"id":3,"name":"Раптор","image":"TestImg.png","isAlive":true,"isActivated":true,"vip":true,"craetionAs":"2020-01-04T13:50:37.294443"},
            null
          ]
        };
      }else{
        let url = "/api/Spa/GetUserInfo";      
        let resp = await fetch(url);           
        if(resp.ok){
          const user = await resp.json();
          for (let index = 0; index < user.slots; index++) {
            const element = user.inventory[index];
            if(!element) user.inventory.push(null);
          }
          this.user = user;
        }else window.location.href = "/";
      }
      this.action = false;
    },
    loadingMessage(){
      this.message = {
        tittle: 'Подождите',
        text: 'Идет загрузка...',
        btn1: {
          text: '',
          action: ()=>{ this.dialog = false}
        },
        btn2: {
          text: '',
          action: ()=>{ this.dialog = false}
        }
      }
    },
    infoMessage(message, error = true){
      this.message = {
        tittle: error ? 'Ошибка':'Информация',
        text: message,
        btn1: {
          text: 'Ок',
          action: ()=>{ 
            this.dialog = false;
            this.loadingMessage();
          }
        },
        btn2: {
          text: '',
          action: ()=>{ this.dialog = false}
        }
      }
      //window.console.log(this.message)
      this.dialog = true;
    },
    confirmMessage(message, action){
      this.message = {
        tittle: 'Подтверждение',
        text: message,
        btn1: {
          text: 'Подтвердить',
          action: action
        },
        btn2: {
          text: 'Отмена',
          action: ()=>{ this.dialog = false}
        }
      }
      this.dialog = true;
    }
  },
  components:{
    Sidebar,
    Body,
    Dialog
  },
  mounted(){
    this.getData();
  }
}
</script>

<style lang="scss">
@import './colors';
  .lk{
    width: 100%; 
    min-height:515px;
    background-color: rgba($clr_2, .5);
    border-radius: 5px;
    box-shadow: 2vh 2vh 4vh 1px $clr_2;
    border: 1px solid $clr_3;
    display: flex;
    margin: 10vh 0;
  }  
  .disabled{
    overflow: hidden;
  }
</style>


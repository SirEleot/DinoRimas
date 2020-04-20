<template>
  <div class="lk row">
    <Dialog v-show="dialog" :message="message" />
    <Sidebar class="col-xl-3" :user="user"/>
    <Body class="col-xl-9" :inventory="user.inventory" @onAction="actionBegine" :price="price" :server="user.server" :timer="user.deactivaionTime" />
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
      user:{
        "id":1,
        "steamid":"76561198208390417",
        "profileName":"SirEleot",
        "profileImg":"https://steamcdn-a.akamaihd.net/steamcommunity/public/images/avatars/80/80b9eaafc8aa816019c1678e20430374a98c300a_full.jpg",
        "balance":100000,
        "server":1,
        "changeOnServer":0,
        "slot":3,
        "deactivaionTime": 300,
        "inventory":[
          {
            "id":3,
            "name": "RexAdultS",
            "server":0,
            "characterClass":"RexAdultS",
            "location_Isle_V3": "X=-324573.22 Y=151895.03 Z=-65705.19",
            "growth": "1.0",
            "bGender": true,
            "bIsResting":false,
            "bBrokenLegs":false,
            "skinPaletteSection1":0,
            "skinPaletteSection2":0,
            "skinPaletteSection3":0,
            "skinPaletteSection4":0,
            "skinPaletteSection5":0,
            "skinPaletteSection6":0,
            "active":true
          },
          {
            "id":3,
            "name": "RexAdultS",
            "server":0,
            "characterClass":"RexAdultS",
            "location_Isle_V3": "X=-324573.22 Y=151895.03 Z=-65705.19",
            "growth": "1.0",
            "bGender": true,
            "bIsResting":false,
            "bBrokenLegs":false,
            "skinPaletteSection1":0,
            "skinPaletteSection2":0,
            "skinPaletteSection3":0,
            "skinPaletteSection4":0,
            "skinPaletteSection5":0,
            "skinPaletteSection6":0,
            "active": false
          }
        ]
      },
      action: true,
      dialog: false,
      disactivate: false,
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
        sex: 10000,
        position: 10000,
        slot: 10000,
        grow: 10000
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
        case 'timer':
          msg = `Необходимо подождать пока закончится таймер`;
          return this.infoMessage(msg, false);

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

        case 'desactivate':
          if(!this.disactivate)return this.infoMessage("У вас нет свободного слота. Необходимо удалить какого-то динозавра или приобрести дополнительный слот", false);
          else {
            msg = `Это действие будет завершено через 6 минут. В течении этого времени нельзя заходить на сервер. Вы хотите деактивировать этого динозавра?`
            action = async ()=>{
              let url = `/api/Spa/DisactivateDino?id=${id}`;      
              this.request(url);
            } 
          }
          break;

        case 'serverSelect':
          if(this.user.server == id) return;
          msg = `Перейти к серверу ${id + 1}?`;
          action = async ()=>{
            let url = `/api/Spa/SelectServer?id=${id}`;      
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
          msg = `Стоимость смены пола для динозавров progression: ${this.price.sex * 2} DC, для  survival: ${this.price.sex } DC. Желаете сменить пол этому диназавру?`
          action = async ()=>{
            let url = `/api/Spa/ChangeSex?id=${id}`;      
            this.request(url);
          } 
          break;

        case 'grow':
          if(this.price.grow > this.user.balance) return this.infoMessage("Недостаточно средств");
          msg = `Стоимость апгрейда динозавра до следующей стадии ${this.price.grow} DC. Желаете перейти на следующую стадию?`
          action = async ()=>{
            let url = `/api/Spa/GrowDino?id=${id}`;      
            this.request(url);
          } 
          break;

        case 'delete':
          msg = `Если данный динозавр является активным - действие будет завершено через 6 минут. Желаете удалить навсегда этого диназавра?`
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
      } else {
        this.infoMessage('Неверный ответ от сервера');
      }      
      this.action = false;
    },
    async getData(){
      if(this.isDev()) {
        this.disactivate = (this.user.inventory.length < this.user.slot); 
        //window.console.log("Check", this.disactivate);
        
        for (let index = 0; index < this.user.slot; index++) {
          const element = this.user.inventory[index];
          if(!element) this.user.inventory.push(null);
        }
        this.action = false;
      }else{        
        let url = "/api/Spa/GetUserInfo";      
        let resp = await fetch(url);           
        if(resp.ok){
          const user = await resp.json();
          if(user.banned) return window.location.href = "/";
          this.disactivate = (user.inventory.length < user.slot); 
          for (let index = 0; index < user.slot; index++) {
            const element = user.inventory[index];
            if(!element) user.inventory.push(null);
          }                
          this.user = user;        
        //window.console.log("Check", JSON.stringify(this.user));
        }else window.location.href = "/";
        url = url = "/api/Spa/GetPrice";
        resp = await fetch(url); 
        if(resp.ok) this.price = await resp.json();
        else window.location.href = "/";
        this.action = false;
      }      
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
    min-height:615px;
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


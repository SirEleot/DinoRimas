<template>
    <div class="lk-sidebar">
         <div class="lk-menu">
             <div class="lk-img">                 
                <img :src="user.profileImg" alt="logo">
             </div>
             <div class="lk-info">{{ user.profileName }}</div>
             <div class="lk-info">{{ user.steamid }}</div>
             <div class="lk-balance">{{ user.balance }} DC</div>
            <div class="button" @click="goTo('/User/Donate')">Пополнить баланс</div>
            <div class="button"  @click="goTo('/User/SignOut')">Выход</div>
         </div>
    </div>
</template>

<script>
export default {
    props:['user'],
    methods: {
        goTo(path){
            window.location.href=path;
        }
    },
    data() {
        return {
            
        }
    },
}
</script>


<style lang="scss">
@import '../colors';
.lk{
    &-sidebar{
        padding: 10px;
        color: $clr_1;
        text-align: center;
    }
    &-img{
        text-align: center;

        margin-top: 15px;
        img{
            width: 50%;
        }
    }
    &-menu{
        min-height: 400px;
        background-color: rgba($clr_2, .5);
        border-radius: 5px;
        box-shadow: 2vh 2vh 4vh 1px $clr_2;
        border: 1px solid $clr_3;
        .button{
            margin: 10px;
            padding: 10px;
            text-align: center;
        }
    }
    &-info{
        margin-top: 15px;
    }
    &-balance{
        color: $clr_4;
        font-weight: bold;
        font-size: 1.2rem;
        margin-top: 20px;
    }
}
</style>
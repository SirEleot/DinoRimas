<template>
    <div class="dialog">
        <div class="info">
            <div class="info-tittle">{{message.tittle}}</div>
            <div class="info-body">
                <div class="info-text">{{message.text}}</div>
                <div class="dialog-buttons">
                    <div class="dialog-btn button" v-show="message.btn1.text != ''" @click="message.btn1.action()">{{message.btn1.text}}</div>
                    <div class="dialog-btn button" v-show="message.btn2.text != ''" @click="message.btn2.action()">{{message.btn2.text}}</div>
                </div>
            </div>
            
        </div>
    </div>
</template>

<script>
export default {
    props: ['message']
}
</script>

<style lang="scss">
    @import '../colors';
    .dialog{
        position: fixed;
        left: 0;
        top: 0;
        display: flex;
        justify-content: center;
        align-items: center;
        width: 100%;
        height: 100%;
        z-index: 500;
        background-color: rgba($clr_2, .8);
        .timer{
            font-weight: bold;
            color: $clr_4;
            font-size: 1.2rem;
            div{
                margin: 0 2vh;
            }
        }
        .info{            
            max-width: 50%;
        }
        &-body{            
            background-color: rgba($clr_2, .7);
            padding: 25px;
        }
        &-buttons{
            display: flex;
            justify-content: space-around;
            align-items: center;
        }
        .info-tittle{
           font-size: 1.2rem;
           font-weight: bold;
           text-transform: uppercase;
        }
        .info-text{
            margin: 20px 0 40px;
            text-align: center;
        }
        .button{
            text-align: center;
            margin: 0 25px;
            padding: 5px 15px;
        }
    }
</style>